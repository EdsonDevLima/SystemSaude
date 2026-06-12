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
  BookingFormState,
  ConsultResponse,
  DayAvailability,
  DoctorAvailability,
  DoctorOption,
  PacientFormState,
  PacientPortalResponse
} from "./types";

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
  const [tema, setTema] = useState<"light" | "dark">(() => {
    if (typeof window === "undefined") {
      return "light";
    }

    const savedTheme = window.localStorage.getItem("cliente-consultas-theme");
    if (savedTheme === "light" || savedTheme === "dark") {
      return savedTheme;
    }

    return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
  });
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
  const [etapaAtiva, setEtapaAtiva] = useState(1);

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", tema);
    window.localStorage.setItem("cliente-consultas-theme", tema);
  }, [tema]);

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
      // Limpa horário selecionado ao trocar de médico
      setForm((current) => ({ ...current, startAt: "", endAt: "" }));

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
      setEtapaAtiva(2);
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
      setEtapaAtiva(3);
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

  function alternarTema() {
    setTema((current) => (current === "light" ? "dark" : "light"));
  }

  return (
    <main className="page-shell">
      <section className="stepper">
        <button
          type="button"
          className={`step-tab ${etapaAtiva === 1 ? "is-active" : ""}`}
          onClick={() => setEtapaAtiva(1)}
        >
          <span>01</span>
          Dados do cliente
        </button>
        <button
          type="button"
          className={`step-tab ${etapaAtiva === 2 ? "is-active" : ""}`}
          onClick={() => pacienteAtual && setEtapaAtiva(2)}
          disabled={!pacienteAtual}
        >
          <span>02</span>
          Selecionar medico
        </button>
        <button
          type="button"
          className={`step-tab ${etapaAtiva === 3 ? "is-active" : ""}`}
          onClick={() => pacienteAtual && setEtapaAtiva(3)}
          disabled={!pacienteAtual}
        >
          <span>03</span>
          Historico
        </button>
      </section>

      {etapaAtiva === 1 && (
        <form className="panel step-panel" onSubmit={identificarPaciente}>
          <span className="section-tag">Etapa 1</span>
          <h2>Dados do cliente</h2>
          <p className="panel-copy">
            Preencha os dados para cadastrar ou identificar o cliente antes de seguir para o agendamento.
          </p>

          <div className="form-grid">
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
          </div>

          <button type="submit" disabled={salvandoPaciente}>
            {salvandoPaciente ? "Salvando..." : "Continuar como paciente"}
          </button>
        </form>
      )}

      {etapaAtiva === 2 && (
        <section className="stack-layout">
          <form className="panel step-panel" onSubmit={agendarConsulta}>
            <span className="section-tag">Etapa 2</span>
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

            {/* Grade de disponibilidade logo abaixo do select de médico */}
            {form.doctorId && (
              <div className="availability-inline">
                {carregandoDisponibilidade && (
                  <div className="empty-state">Carregando horarios do medico...</div>
                )}

                {!carregandoDisponibilidade && disponibilidade && (
                  <>
                    <div className="availability-inline-header">
                      <span className="section-tag">Agenda semanal</span>
                      <p className="panel-copy">
                        Clique em um horario <strong>disponivel</strong> para preencher o agendamento automaticamente.
                        Horarios em vermelho estao ocupados.
                      </p>
                    </div>

                    <div className="availability-grid">
                      {disponibilidade.days.map((day) => (
                        <article className="day-column" key={day.dayOfWeek}>
                          <h3>{day.dayLabel}</h3>

                          {day.slots.length === 0 && (
                            <div className="slot-empty">Sem agenda configurada</div>
                          )}

                          {day.slots.map((slot) => {
                            const slotStart = buildIsoDate(day.dayOfWeek, slot.startTime);
                            const slotEnd = buildIsoDate(day.dayOfWeek, slot.endTime);
                            const isSelected =
                              form.startAt === slotStart &&
                              form.endAt === slotEnd;

                            return (
                              <button
                                key={`${day.dayOfWeek}-${slot.startTime}`}
                                type="button"
                                className={`slot-button ${slot.available ? "is-free" : "is-busy"} ${isSelected ? "is-selected" : ""}`}
                                onClick={() =>
                                  slot.available && selecionarHorario(day, slot.startTime, slot.endTime)
                                }
                                disabled={!slot.available}
                                title={slot.available ? "Clique para selecionar" : "Vaga ocupada"}
                              >
                                <span>{slot.startTime}</span>
                                {slot.available ? (
                                  <small className="slot-label-free">Disponivel</small>
                                ) : (
                                  <small className="slot-label-busy">Vaga ocupada</small>
                                )}
                              </button>
                            );
                          })}
                        </article>
                      ))}
                    </div>
                  </>
                )}
              </div>
            )}

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

            <button type="submit" disabled={salvando || !form.doctorId || !pacienteAtual || !form.startAt}>
              {salvando ? "Agendando..." : "Agendar consulta"}
            </button>
          </form>
        </section>
      )}

      {(erro || sucesso) && (
        <section className="feedback-stack">
          {erro && <div className="feedback error">{erro}</div>}
          {sucesso && <div className="feedback success">{sucesso}</div>}
        </section>
      )}

      {etapaAtiva === 3 && (
        <section className="panel consult-list">
          <div className="list-header">
            <div>
              <span className="section-tag">Historico</span>
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
                Nenhuma consulta carregada ainda. Faca uma busca para visualizar os dados.
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
      )}
    </main>
  );
}