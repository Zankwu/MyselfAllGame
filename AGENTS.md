# FightX — 项目指南

## 技术栈

- **引擎**: Godot 4.7 (C#) + `.NET 10.0`
- **渲染**: GL 兼容 + D3D12 (Windows), 最近邻纹理过滤
- **分辨率**: 视口 100×64 → 窗口 1000×640 (整数缩放, viewport stretch)
- **物理**: Godot 内置 2D 物理层 (Player=层3, Enemy=层4, World=层5)

## 构建与运行

```bash
dotnet build          # 构建 C# 项目
```

在 Godot 4.7 编辑器中打开 `project.godot` 并运行主场景。

## 架构

### 场景树

```
World (Node2D) → World.cs
├── Stage (Node2D) → stage.tscn (背景 + 世界碰撞边界)
├── Player → player.tscn → charater.tscn
│   └── Character (CharacterBody2D) → Character.cs → Player.cs
│       ├── skin (Sprite2D) — 角色精灵
│       ├── shadow (Sprite2D) — 阴影
│       ├── CollisionShape2D (CapsuleShape2D)
│       ├── animation (AnimationPlayer)
│       └── stateMachine (Node) — StateMachine.cs (运行时热替换脚本)
└── camera (Camera2D) — 跟随玩家
```

### 状态机 (核心模式)

采用 **运行时脚本替换 (`SetScript()`)** 实现状态切换，**不是**标准节点切换模式：

1. **`StateMachine.cs`** — 基类, 提供 `ChangeStateBegin()` / `ChangeStateEnd()` 虚方法
2. **`PreState.cs`** — Autoload 单例, 缓存 `idle` / `walk` 脚本引用 (`PreState.instance`)
3. **`Idle.cs`** / **`Walk.cs`** — 具体状态, 通过 `GetParent() as Character` 获取角色引用
4. **`Character.ChangeState(Script, string)`** — 切换状态: 调 `ChangeStateEnd()` → `SetScript()` → `ChangeStateBegin()`

> ⚠️ 新状态类必须继承 `StateMachine`, 并通过 `PreState.instance` 注册脚本引用。

### 输入映射

| 动作 | 按键 |
|------|------|
| `up` | ↑ / W |
| `down` | ↓ |
| `left` | ← |
| `right` | → |
| `attack` | X |
| `jump` | Space |

## 编码规范

- **类/文件名**: PascalCase (如 `World.cs`, `Idle.cs`, `StateMachine.cs`)
- **变量**: camelCase (如 `currentState`, `stateMachine`)
- **节点路径**: snake_case (如 `stateMachine`, `leftWall`, `InvisibleWalls`)
- **全局命名空间**: 所有类在全局命名空间中, `public partial class`
- **导出**: 使用 `[Export]` 属性暴露编辑器变量
- **Godot 生命周期**: `override _Ready()`, `_Process(double delta)`, `_EnterTree()`

## 注意事项

1. **资源路径**: 文件夹名是 `accetss/` (拼写错误, 不是 `assets/`), 代码中资源路径使用 `res://accetss/...`
2. **`.csproj` 硬编码路径**: `CodeAnalysisRuleSet` 指向绝对路径 `d:\workPlace\...\ruleset.xml`, 其他机器上可能不存在
3. **缺少 `.gitignore`**: 注意不要提交 `.godot/` 等构建产物到版本控制
4. **像素艺术**: 保持像素对齐, 使用整数坐标, 避免子像素渲染
5. **`Player.cs` 未使用的命名空间**: `System.Reflection.Metadata` 已导入但未使用
