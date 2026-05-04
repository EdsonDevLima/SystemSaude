# Cliente Consultas MF

Microfrontend em React + TypeScript para o proprio paciente:

- consultar as consultas dele por `pacienteId`
- escolher o medico pelo nome e especialidade
- visualizar a grade de horarios disponiveis de segunda a sexta
- clicar em um slot livre para preencher o agendamento
- consumir a API `SystemSaude`

## Rodar o projeto

1. Instale as dependencias:

```bash
npm install
```

2. Configure a URL da API:

```bash
cp .env.example .env
```

Por padrao o projeto usa `VITE_API_URL=/api`, com proxy do Vite para `http://localhost:3000`.
Se sua API estiver em outra porta, ajuste `VITE_API_URL` no `.env`.

3. Inicie o ambiente:

```bash
npm run dev
```

## Observacao

Atualmente a API backend consulta o paciente pelo `GUID`. Como ainda nao existe autenticacao nem endpoint de login, o microfrontend usa esse identificador como chave de acesso para listar e criar consultas do proprio paciente.

## Endpoints usados

- `GET /api/Consults/pacient/{pacientId}`
- `POST /api/Consults`
- `GET /api/ClientPortal/doctors`
- `GET /api/ClientPortal/doctors/{doctorId}/availability`
