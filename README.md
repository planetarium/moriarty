# Moriarty

## Overview

Moriarty is an experimental text-based game, guessing murderer of fictional crime scene.
Content creation and game playing are supported LLM, thorugh [Semantic Kernel][].


[Semantic Kernel]: https://github.com/microsoft/semantic-kernel


## Prerequistes

-  [.NET][] SDK 8.0+
-  [dotnet-ef][]
-  [OpenAI][] API Key


[.NET]: https://dotnet.microsoft.com/en-us/
[dotnet-ef]: https://www.nuget.org/packages/dotnet-ef
[OpenAI]: https://openai.com/


## How to run (for devleopment)

1. Set OpenAI API key to `dotnet user-secrets` as below.
```bash
$ dotnet user-secrets init --project Moriarty.Web/
$ dotnet user-secrets set "OpenAI:ApiKey" "<OPENAI_API_KEY>"
```
2. (Optional) Change path of SQLite database file.
```bash
$ export ConnectionStrings__DefaultConnection="Data Source=<NEW PATH>"
```

3. Initialize database by `dotnet ef`
```bash
$ dotnet-ef --project Moriarty.Web database update
```

4. Build and run
```bash
dotnet run --project Moriarty.Web
```

5. Check the `http://localhost:5229` by your preferred web browser.
