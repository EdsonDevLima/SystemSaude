import { useEffect, useState, type ChangeEvent, type FormEvent } from "react";
import {
  API_URL,
  criarConsulta,
  getConsultasPorPaciente,
  getDisponibilidadeMedico,
  getMedicos,
  registrarPaciente
} from "./services/consultasApi";
import type {
  ConsultResponse,
  DayAvailability,
  DoctorAvailability,
  DoctorOption,
  PacientPortalResponse
} from "./types";

interface BookingFormState {
  doctorId: string;
  startAt: string;
  endAt: string;
  notes: string;
}

interface PacientFormState {
  email: string;
  cpf: string;
  document: string;
  phone: string;
  street: string;
  neighborhood: string;
  state: string;
  country: string;
  complement: string;
}

const initialForm: BookingFormState = {
  doctorId: "",
  startAt: "",
  endAt: "",
  notes: ""
};

const initialPacientForm: PacientFormState = {
  email: "",
  cpf: "",
  document: "",
  phone: "",
  street: "",
  neighborhood: "",
  state: "",
  country: "Brasil",
  complement: ""
};

function formatDateTime(value: string) {
  if (!value) {
    return "-";
  }

  return new Date(value).toLocaleString("pt-BR", {
    dateStyle: "short",
    timeStyle: "short"
  });
}

function statusLabel(status: string) {
  const labels: Record<string, string> = {
    scheduled: "Agendada",
    completed: "Concluida",
    cancelled: "Cancelada"
  };

  return labels[status] || status;
}

function sortConsultas(items: ConsultResponse[]) {
  return [...items].sort(
    (a, b) => new Date(a.startAt).getTime() - new Date(b.startAt).getTime()
  );
}

function buildIsoDate(dayOfWeek: number, time: string) {
  const now = new Date();
  const today = now.getDay();
  const normalizedToday = today === 0 ? 7 : today;
  let diff = dayOfWeek - normalizedToday;

  if (diff < 0) {
    diff += 7;
  }

  const target = new Date(now);
  target.setHours(0, 0, 0, 0);
  target.setDate(now.getDate() + diff);

  const [hours, minutes] = time.split(":").map(Number);
  target.setHours(hours, minutes, 0, 0);

  const offset = target.getTimezoneOffset();
  const localDate = new Date(target.getTime() - offset * 60_000);
  return localDate.toISOString().slice(0, 16);
}

export default function App() {
  const [form, setForm] = useState<BookingFormState>(initialForm);
  const [pacientForm, setPacientForm] = useState<PacientFormState>(initialPacientForm);
  const [pacienteAtual, setPacienteAtual] = useState<PacientPortalResponse | null>(null);
  const [consultas, setConsultas] = useState<ConsultResponse[]>([]);
  const [medicos, setMedicos] = useState<DoctorOption[]>([]);
  const [disponibilidade, setDisponibilidade] = useState<DoctorAvailability | null>(null);
  const [carregandoConsultas, setCarregandoConsultas] = useState(false);
  const [carregandoMedicos, setCarregandoMedicos] = useState(true);
  const [carregandoDisponibilidade, setCarregandoDisponibilidade] = useState(false);
  const [salvandoPaciente, setSalvandoPaciente] = useState(false);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState("");
  const [sucesso, setSucesso] = useState("");

  useEffect(() => {
    async function loadDoctors() {
      try {
        const data = await getMedicos();
        setMedicos(data);
      } catch (requestError) {
        setErro(requestError instanceof Error ? requestError.message : "Erro ao carregar medicos.");
      } finally {
        setCarregandoMedicos(false);
      }
    }

    void loadDoctors();
  }, []);

  useEffect(() => {
    async function loadAvailability() {
      if (!form.doctorId) {
        setDisponibilidade(null);
        return;
      }

      setCarregandoDisponibilidade(true);

      try {
        const data = await getDisponibilidadeMedico(form.doctorId);
        setDisponibilidade(data);
      } catch (requestError) {
        setDisponibilidade(null);
        setErro(requestError instanceof Error ? requestError.message : "Erro ao carregar disponibilidade.");
      } finally {
        setCarregandoDisponibilidade(false);
      }
    }

    void loadAvailability();
  }, [form.doctorId]);

  function handleChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) {
    const { name, value } = event.target;
    setForm((current) => ({
      ...current,
      [name]: value,
      ...(name === "doctorId" ? { startAt: "", endAt: "" } : {})
    }));
  }

  function handlePacientChange(event: ChangeEvent<HTMLInputElement>) {
    const { name, value } = event.target;
    setPacientForm((current) => ({
      ...current,
      [name]: value
    }));
  }

  async function identificarPaciente(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setErro("");
    setSucesso("");
    setSalvandoPaciente(true);

    try {
      const paciente = await registrarPaciente({
        email: pacientForm.email,
        cpf: pacientForm.cpf,
        document: pacientForm.document,
        phone: pacientForm.phone,
        address: {
          street: pacientForm.street,
          neighborhood: pacientForm.neighborhood,
          state: pacientForm.state,
          country: pacientForm.country,
          complement: pacientForm.complement
        }
      });

      setPacienteAtual(paciente);
      setCarregandoConsultas(true);
      const data = await getConsultasPorPaciente(paciente.id);
      setConsultas(sortConsultas(data));
      setSucesso("Paciente identificado com sucesso. Agora voce pode escolher um horario.");
    } catch (requestError) {
      setConsultas([]);
      setPacienteAtual(null);
      setErro(requestError instanceof Error ? requestError.message : "Erro ao identificar paciente.");
    } finally {
      setSalvandoPaciente(false);
      setCarregandoConsultas(false);
    }
  }

  async function agendarConsulta(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setErro("");
    setSucesso("");
    setSalvando(true);

    try {
      await criarConsulta({
        doctorId: form.doctorId,
        pacientId: pacienteAtual?.id ?? "",
        startAt: form.startAt,
        endAt: form.endAt,
        status: "scheduled",
        notes: form.notes
      });

      setSucesso("Consulta agendada com sucesso.");
      const data = await getConsultasPorPaciente(pacienteAtual?.id ?? "");
      setConsultas(sortConsultas(data));

      if (form.doctorId) {
        const refreshedAvailability = await getDisponibilidadeMedico(form.doctorId);
        setDisponibilidade(refreshedAvailability);
      }

      setForm((current) => ({
        ...current,
        startAt: "",
        endAt: "",
        notes: ""
      }));
    } catch (requestError) {
      setErro(requestError instanceof Error ? requestError.message : "Erro ao agendar consulta.");
    } finally {
      setSalvando(false);
    }
  }

  function selecionarHorario(day: DayAvailability, startTime: string, endTime: string) {
    setForm((current) => ({
      ...current,
      startAt: buildIsoDate(day.dayOfWeek, startTime),
      endAt: buildIsoDate(day.dayOfWeek, endTime)
    }));
    setSucesso("");
    setErro("");
  }

  return (
    <main className="page-shell">
      <section className="hero-card">
        <div>
          <h1>Portal de consultas do cliente</h1>
          <p>
            O cliente consulta seus agendamentos, escolhe o medico pelo nome e seleciona
            um horario disponivel entre segunda e sexta de forma mais simples.
          </p>
        </div>

        <div className="api-badge">
          <span>API alvo</span>
          <strong>{API_URL}</strong>
        </div>
      </section>

      <section className="grid-layout">
        <form className="panel" onSubmit={identificarPaciente}>
          <h2>Dados do paciente</h2>
          <p className="panel-copy">
            Preencha os dados para cadastrar ou identificar o paciente antes de agendar.
          </p>

          <label>
            E-mail
            <input
              name="email"
              type="email"
              placeholder="cliente@email.com"
              value={pacientForm.email}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            CPF
            <input
              name="cpf"
              type="text"
              placeholder="000.000.000-00"
              value={pacientForm.cpf}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            Documento
            <input
              name="document"
              type="text"
              placeholder="RG ou outro documento"
              value={pacientForm.document}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            Telefone
            <input
              name="phone"
              type="text"
              placeholder="(81) 99999-9999"
              value={pacientForm.phone}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            Rua
            <input
              name="street"
              type="text"
              placeholder="Rua do paciente"
              value={pacientForm.street}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            Bairro
            <input
              name="neighborhood"
              type="text"
              placeholder="Bairro"
              value={pacientForm.neighborhood}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            Estado
            <input
              name="state"
              type="text"
              placeholder="PE"
              value={pacientForm.state}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            Pais
            <input
              name="country"
              type="text"
              placeholder="Brasil"
              value={pacientForm.country}
              onChange={handlePacientChange}
              required
            />
          </label>

          <label>
            Complemento
            <input
              name="complement"
              type="text"
              placeholder="Casa, apartamento, referencia"
              value={pacientForm.complement}
              onChange={handlePacientChange}
              required
            />
          </label>

          <button type="submit" disabled={salvandoPaciente}>
            {salvandoPaciente ? "Salvando..." : "Continuar como paciente"}
          </button>
        </form>

        <form className="panel" onSubmit={agendarConsulta}>
          <h2>Agendar consulta</h2>
          <p className="panel-copy">
            Escolha o medico, toque em um horario livre na grade semanal e confirme.
          </p>

          {pacienteAtual && (
            <div className="patient-summary">
              <strong>{pacienteAtual.email}</strong>
              <span>CPF: {pacienteAtual.cpf}</span>
              <span>Telefone: {pacienteAtual.phone}</span>
            </div>
          )}

          <label>
            Medico
            <select
              name="doctorId"
              value={form.doctorId}
              onChange={handleChange}
              required
              disabled={carregandoMedicos || !pacienteAtual}
            >
              <option value="">
                {carregandoMedicos ? "Carregando medicos..." : "Selecione um medico"}
              </option>
              {medicos.map((medico) => (
                <option key={medico.id} value={medico.id}>
                  {medico.name} - {medico.speciality}
                </option>
              ))}
            </select>
          </label>

          <label>
            Inicio da consulta
            <input
              name="startAt"
              type="datetime-local"
              value={form.startAt}
              onChange={handleChange}
              required
            />
          </label>

          <label>
            Fim da consulta
            <input
              name="endAt"
              type="datetime-local"
              value={form.endAt}
              onChange={handleChange}
              required
            />
          </label>

          <label>
            Observacoes
            <textarea
              name="notes"
              rows={4}
              placeholder="Motivo da consulta, sintomas ou pedido especial"
              value={form.notes}
              onChange={handleChange}
            />
          </label>

          <button type="submit" disabled={salvando || !form.doctorId || !pacienteAtual}>
            {salvando ? "Agendando..." : "Agendar consulta"}
          </button>
        </form>
      </section>

      {(erro || sucesso) && (
        <section className="feedback-stack">
          {erro && <div className="feedback error">{erro}</div>}
          {sucesso && <div className="feedback success">{sucesso}</div>}
        </section>
      )}

      <section className="panel availability-panel">
        <div className="list-header">
          <div>
            <h2>Horarios disponiveis</h2>
            <p className="panel-copy">
              Grade semanal de segunda a sexta. Clique em um horario livre para preencher o agendamento.
            </p>
          </div>
          {disponibilidade && (
            <span className="counter">
              {disponibilidade.doctorName} - {disponibilidade.speciality}
            </span>
          )}
        </div>

        {!pacienteAtual && (
          <div className="empty-state">Cadastre ou identifique o paciente para liberar os horarios.</div>
        )}

        {pacienteAtual && !form.doctorId && (
          <div className="empty-state">Selecione um medico para visualizar a disponibilidade.</div>
        )}

        {pacienteAtual && form.doctorId && carregandoDisponibilidade && (
          <div className="empty-state">Carregando horarios do medico...</div>
        )}

        {pacienteAtual && disponibilidade && !carregandoDisponibilidade && (
          <div className="availability-grid">
            {disponibilidade.days.map((day) => (
              <article className="day-column" key={day.dayOfWeek}>
                <h3>{day.dayLabel}</h3>
                {day.slots.length === 0 && (
                  <div className="slot-empty">Sem agenda configurada</div>
                )}
                {day.slots.map((slot) => {
                  const isSelected =
                    form.startAt.endsWith(slot.startTime) && form.endAt.endsWith(slot.endTime);

                  return (
                    <button
                      key={`${day.dayOfWeek}-${slot.startTime}`}
                      type="button"
                      className={`slot-button ${slot.available ? "is-free" : "is-busy"} ${isSelected ? "is-selected" : ""}`}
                      onClick={() => selecionarHorario(day, slot.startTime, slot.endTime)}
                      disabled={!slot.available}
                    >
                      <span>{slot.startTime}</span>
                      <small>{slot.available ? "Disponivel" : "Ocupado"}</small>
                    </button>
                  );
                })}
              </article>
            ))}
          </div>
        )}
      </section>

      <section className="panel consult-list">
        <div className="list-header">
          <div>
            <h2>Historico de consultas</h2>
            <p className="panel-copy">
              Resultado filtrado por paciente, com medico, horario e status.
            </p>
          </div>
          <span className="counter">{consultas.length} registros</span>
        </div>

        <div className="cards">
          {consultas.length === 0 ? (
            <div className="empty-state">
              Nenhuma consulta carregada ainda. Faça uma busca para visualizar os dados.
            </div>
          ) : (
            consultas.map((consulta) => (
              <article className="consult-card" key={consulta.id}>
                <div className="consult-top">
                  <strong>{consulta.doctorName || "Medico nao informado"}</strong>
                  <span className={`status-chip status-${consulta.status}`}>
                    {statusLabel(consulta.status)}
                  </span>
                </div>

                <p>
                  <span>Paciente:</span> {consulta.pacientEmail || consulta.pacientId}
                </p>
                <p>
                  <span>Inicio:</span> {formatDateTime(consulta.startAt)}
                </p>
                <p>
                  <span>Fim:</span> {formatDateTime(consulta.endAt)}
                </p>
                <p>
                  <span>Observacoes:</span> {consulta.notes || "Sem observacoes"}
                </p>
              </article>
            ))
          )}
        </div>
      </section>
    </main>
  );
}
