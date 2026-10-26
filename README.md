# Utrecht Cameras

Solution for the [Keep Talking and Everybody Codes](https://github.com/infi-nl/everybody-codes) assignment (English translation of the task).
It has three parts built on one shared data set (`Back-end/data/cameras-defb.csv`): a CLI search tool, a REST API, and a React web app with a map and a table.

## Structure

```
Back-end/
  data/cameras-defb.csv              the single copy of the data
  src/CameraExplorer.Core            Camera model, CameraService (search/order), repository interface
  src/CameraExplorer.Infrastructure  CSV parser and CSV-backed repository
  src/CameraExplorer.Api             REST API (ASP.NET Core minimal API)
  src/CameraExplorer.Cli             "Search" command-line tool
  tests/CameraExplorer.Tests         unit and API integration tests (xunit)
Front-end/                           React + TypeScript (Vite)
```

`Core` depends on nothing. `Infrastructure`, `Api` and `Cli` depend on it, so the CLI and the API share the same loading and search logic.

## Requirements

- .NET 8 SDK
- Node.js 20.19+ or 22.12+

## Run it

**CLI**

```
cd Back-end
dotnet run --project src/CameraExplorer.Cli -- --name Neude
```

**API** (listens on http://localhost:5080, Swagger UI at `/swagger` in Development)

```
cd Back-end
dotnet run --project src/CameraExplorer.Api
```

`GET /api/cameras` returns all cameras ordered by number. `GET /api/cameras?name=Neude` filters them (case-insensitive, partial match).

**Web app** (needs the API running; open http://localhost:5173)

```
cd Front-end
npm install
npm run dev
```

The API address comes from `VITE_API_URL` in `Front-end/.env.development`.

**Tests**

```
cd Back-end && dotnet test
cd Front-end && npm test
```

## Decisions and trade-offs

- **Bad data is skipped, loudly.** Line 54 of the CSV is not a camera but an export error message (`ERROR 1207 ...`). The parser skips invalid rows and reports each one with its line number and reason. The CLI prints a warning and the API logs one at startup. Crashing would take down 88 valid cameras, while silently skipping would hide a data problem.
- **The camera number is parsed from the name.** The CSV has no number column. The number sits after a prefix such as `UTR-CM-`, and some names use a hyphen after it (`UTR-CM-544-Janskerkhof`), so the pattern does not depend on what follows the digits.
- **Invariant culture everywhere.** Coordinates use a dot as decimal separator. Parsing and printing ignore the machine's locale, and there are tests that run under `ro-RO`.
- **Search logic lives in `Core`**, not in the CLI or the API, so both behave identically.
- **Column grouping lives in the front-end.** It is a presentation concern, so the API stays a plain data endpoint. The columns are a data array in `domain/cameraColumns.ts`.
- **The assignment says "five columns" but defines four rules** (below 600, 600-699, 700-799, all others). I implemented the four listed rules. Adding a fifth is a one-line change in the array.
- **The 600-699 column is empty** because the data contains no cameras in that range. It is still shown because the columns are defined by the number ranges, not by the data.
- **Data loading is lazy and cached in memory.** 88 rows do not need a database. The repository is behind an interface, so a different source would only touch `Infrastructure`.

## Not done / next steps

- No runtime validation of the API response in the front-end (a schema validator such as zod would be the next step).
- No pagination or marker clustering. Not needed for 88 cameras.
- The map keeps the fixed Utrecht center from the task instead of fitting to the markers.

## Use of AI

I used Claude as a pair-programming partner for the architecture, the code and the tests, and I ran, reviewed and debugged every step myself, including the real-data problems we found along the way, such as the hyphenated camera names and the error line in the CSV.
AI also helped with creating this md file with the documentation.
