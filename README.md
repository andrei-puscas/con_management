# con_management

Aplicație de management lucrări și echipe pentru firme de construcții (proiect licență). Backend .NET 10 + frontend Angular cu Tailwind.

## Cerințe

- **Node.js 18+** (recomandat 20 LTS) – pentru frontend și `npm start`
- **.NET 10 SDK** – pentru backend

Verifică versiunile: `node -v` și `dotnet --version`.

### Actualizare Node.js (Windows)

1. **Descărcare directă:** [nodejs.org](https://nodejs.org/) – alege varianta **LTS** (ex. 20.x), rulează installer-ul și repornește terminalul.
2. **Cu nvm-windows** (mai multe versiuni pe același PC): [nvm-windows](https://github.com/coreybutler/nvm-windows) – după instalare: `nvm install 20` apoi `nvm use 20`.

După actualizare: `node -v` ar trebui să arate v18.x sau v20.x.

## Rulare rapidă

Din directorul principal:

```bash
npm install
npm start
```

Pornește **doar frontend-ul** (http://localhost:4200). Oprire cu Ctrl+C.

- **API (backend):** `npm run api` sau `cd backend && dotnet run` → http://localhost:5000
- **Frontend + API:** `npm run start:all` (pornește ambele în paralel)

**Prima rulare:** în `backend` rulează `dotnet restore`, în `frontend` rulează `npm install` (sau rulezi manual când e nevoie)