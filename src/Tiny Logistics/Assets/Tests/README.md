# Tests

Open **Window > General > Test Runner**, select **EditMode**, then run all tests.

The suite uses Unity's predefined `Assembly-CSharp-Editor` assembly. Most tests run synchronously in Edit Mode; the `Startup` category explicitly enters Play Mode to exercise the real Boot scene, loading states, UI input and session restart.

| Folder | Coverage |
| --- | --- |
| Unit/Configs | Config validation and missing repository handling |
| Unit/Infrastructure | UI event dispatch |
| Unit/Stockpile | Resource accounting |
| Integration/Infrastructure | Loading real config assets |
| Integration/Simulation | ECS systems, gameplay services and feature pipelines |
| Integration/Persistence | Snapshot round trips, save scheduling and real file storage |
| Integration/Presentation | UI notifications, prefab binding and ECS/view lifetime |
| Integration/Startup | Boot, mouse input, save restoration and repeated restart |
| Support | Shared fixtures, in-memory storage and isolated input/save helpers |

Use the `Unit` or `Integration` category in Test Runner to run either group. Existing domain categories remain available. All `Startup` tests belong to `Integration`; exclude `Startup` for integration checks without entering Play Mode. View tests load real prefabs and require a graphics device.

Unit tests exercise an individual class without loading project assets, scenes, a DI container or ECS fixtures. Config validation creates temporary ScriptableObjects directly and still runs in Edit Mode. Integration tests cover collaborating runtime components or external resources such as project assets and the file system.

Use NSubstitute for simple interface stubs and interaction checks. Unit tests substitute dependencies such as `IAssetRepository`, `IUiDispatcher` and event listeners. Shared ECS fixtures also substitute the config provider; gameplay services, systems and prefab assets remain real.

`GameTestContext` uses fresh ECS contexts, controlled time and in-memory save files. Prefabs are loaded only when a test requests `includeViews: true`. Startup tests override the save directory through VContainer and use a temporary folder; player save files are never replaced. Teardown restores time scale and simulated mouse input settings.

Rendering and screenshot checks are intentionally outside this suite.

Test names follow `MethodName_StateUnderTest_ExpectedBehavior`, or `MethodName_ExpectedBehavior` when no condition is needed. Scenarios spanning several systems use the operation name, such as `SaveAndLoadProgress`, as the first block.
