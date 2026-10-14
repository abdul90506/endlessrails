# Compile Check (offline, no Unity needed)

Verifies that ALL game code (Assets/Scripts + Assets/Editor) compiles cleanly
against Unity 6 API stubs using the real C# compiler (Roslyn, C# 9 = Unity 6
language level). Catches CS0029/CS1061-style type errors BEFORE they waste a
cloud build.

## Run

    bash Tools/compile_check/compile_check.sh

Requires the .NET SDK 8 (`dotnet`). Output must say `Build succeeded. 0 Error(s)`.

## Notes

- Stubs.cs / Stubs2.cs are hand-maintained minimal Unity API stubs. If you use a
  NEW Unity API member, add it to the stubs (or the check will flag it).
- This is a compile check only - it does not run the game.
