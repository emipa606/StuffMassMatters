# GitHub Copilot Instructions for "Stuff Mass Matters" RimWorld Mod

## Mod Overview and Purpose

**Mod Name:** Stuff Mass Matters  
**Author:** Mlie  
**PackageId:** Mlie.StuffMassMatters  

"Stuff Mass Matters" is a RimWorld mod that introduces a more realistic weight system for items crafted from various materials. The mod addresses the inconsistency where items crafted from different materials (referred to as "stuff") have the same mass, regardless of the material's inherent weight. By modifying item mass based on the material used, it ensures that heavy materials like Uranium make items significantly heavier than lighter materials such as Bamboo. This change affects all items that can be made from different stuff, including apparel, some weapons, and buildings.

## Key Features and Systems

- **Mass Adjustment:** Calculates and applies relative mass to crafted items based on the material.
- **Movement Speed Modification:** Allows negative movement speed penalties to be affected by item weight, such as heavier armor reducing movement speed more significantly.
- **Compatibility with Companion Mods:** Works seamlessly with "Change Stuff Properties" mod to further modify weight values.

## Coding Patterns and Conventions

- **Project Structure:** The mod consists of C# files organized under the `StuffMassMatters` namespace.
- **Type Naming:** Type names follow PascalCase, e.g., `Main`, `StatExtension_GetStatValue`.
- **Method Naming:** Member methods also follow PascalCase, e.g., `CalculateRelativeMass`, `Postfix`.
- **Code Readability:** Code should be well-documented with comments explaining complex logic or important decisions.

## XML Integration

- Currently, the mod does not involve XML files. However, XML-based Defs are typical in RimWorld modding to define game assets and should be used if further expansion into game assets is desired.

## Harmony Patching

The mod uses Harmony, a popular library for modifying existing code in RimWorld:
- **Harmony Setup:** The mod has `dependencies` on `brrainz.harmony`, indicating its use of Harmony patches.
- **Patch Types and Members:** Includes Postfix and Prefix methods to extend existing methods' behavior. For example:
  - `StatExtension_GetStatValue` and `StatExtension_GetStatValueAbstract` use `Postfix` to adjust the stats after the game's original logic occurs.
  - `StatWorker_StatOffsetFromGear` applies a `Prefix` to modify functionality before the game's original code executes.

## Suggestions for Copilot

To fully leverage GitHub Copilot's AI capabilities, consider the following for this project:

1. **Function Stubbing:** Use Copilot to generate stub methods to handle new features or debugging outputs.
2. **Pattern Recognition:** Allow Copilot to suggest patches for similar patterns found in other mods, potentially reducing redundant coding efforts.
3. **Boilerplate Code:** Utilize Copilot to create repetitive code structures like Harmony patch boilerplates.
4. **Enhanced Documentation:** Ask Copilot for concise inline comments that explain each function's purpose or any complex logic, which can aid in both user understanding and future enhancement of mod features.
5. **Refactoring Assistance:** Let Copilot suggest code refactoring opportunities to improve performance or readability.

Utilizing these guidelines will not only enhance the mod’s functionality but also streamline development through effective use of AI assistance.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

