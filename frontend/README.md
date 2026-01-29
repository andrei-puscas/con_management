# Frontend – ConManagement

Angular 19+, Tailwind CSS v4, Zard UI (componente). Comunicare cu API .NET (JWT).

## Configurare

- **API URL**: în `src/app/core/auth.service.ts` – `http://localhost:5000/api`. Schimbă dacă API-ul rulează pe alt port.
- **Tailwind CSS v4**: Configurat cu PostCSS (`postcss.config.json`). Import în `src/styles.css`. Folosește `.npmrc` cu `legacy-peer-deps=true` pentru compatibilitate cu Angular DevKit.
- **Zard UI**: `components.json` și path aliase (`@/*`) sunt setate. După `npm install`, poți adăuga componente:
  ```bash
  # Adaugă o singură componentă
  npm run zard:add button
  
  # SAU adaugă TOATE componentele Zard UI (45+ componente)
  # Rulează secvențial: accordion, alert, avatar, badge, button, card, dialog, input, table, etc.
  npm run zard:add-all
  ```
  Vezi [zardui.com/docs](https://zardui.com/docs) pentru lista completă.

## Comenzi

```bash
npm install
npm start
```

Aplicația rulează la http://localhost:4200

## Pagini

Toate paginile folosesc **componente Zard UI** (card, button, input, table, alert, loader):

- **/login** – Autentificare cu email și parolă (Zard Card + Input + Button). După succes → redirect la /dashboard.
- **/dashboard** – Dashboard principal (Zard Card) cu statistici placeholder pentru Proiecte, Șantiere, Echipe.
- **/users** – Listă utilizatori (Zard Table + Loader + Alert). Protejat, doar Admin. Apel GET /api/users cu JWT.

**Navigare:** Header cu butoane Zard (Dashboard, Utilizatori, Deconectare) - vizibilitate dinamică bazată pe autentificare și rol.

**Auth:** 
- **Interceptor HTTP:** Atașează JWT la toate request-urile; la 401 → logout și redirect la /login.
- **AuthGuard:** Protejează rutele `/dashboard` și `/users` - redirecționează la `/login` dacă nu ești autentificat.

**Cont admin (backend):** `admin@conmanagement.local` / `Admin123!`
