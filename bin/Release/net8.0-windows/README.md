# NotifyIsland2

ClassIsland 2 的提醒功能与第三方通知服务集成，通过 HTTP API 触发提醒。

## 使用方式

1. 安装插件并重启 ClassIsland
2. 打开 **调试菜单** → **NotifyIsland2**
3. 配置监听地址和端口（默认 `localhost:1379`）
4. 打开 **启用 API 服务** 开关，搞定！🎉
5. （推荐）设置 API 密钥，配置完毕后关闭设置页面

> ⚠️ 监听地址设为 `*` 或 `+` 需以管理员身份运行 ClassIsland。

## 快速编译

双击根目录下的 `build.bat` 即可自动清理、编译并打包 CIPX。编译后用 `quicktest.bat` 验证。

## 接口说明

| 项目 | 值 |
|------|-----|
| 端点 | `POST /api/notify` |
| Content-Type | `application/json` |
| 认证 | `Authorization: Bearer <token>`（可选） |

### 请求字段

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `title` | string | 是 | 遮罩文字 |
| `title_duration` | int | 是 | 标题显示时长（秒） |
| `content` | string | 是 | 正文内容 |
| `content_duration` | int | 是 | 正文显示时长（秒） |
| `title_voice` | string | 否 | 标题语音文字，默认用 `title` |
| `content_voice` | string | 否 | 正文语音文字，默认用 `content` |
| `sound_enabled` | bool | 否 | 是否播放提示音 |
| `effect_enabled` | bool | 否 | 是否启用特效 |

### 示例 ヾ(＾∇＾)

```bash
curl -X POST http://localhost:1379/api/notify \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer 1145141919810" \
  -d '{
    "title": "课间提醒",
    "title_duration": 3,
    "content": "哼哼哼啊啊啊啊啊",
    "content_duration": 10,
    "sound_enabled": true,
    "effect_enabled": true
  }'
```

成功响应：

```json
{"success":true,"status":200,"message":"[200]已推送到ClassIsland"}
```

## 致谢 (≧▽≦)

- **tuanzi_awa** — 原始项目作者，感谢为 ClassIsland 社区带来的优秀插件
- **Milkyweigh** — 瞎几把乱改，优化打包
- **Deepseek V4 Pro** — 完成 v1→v2 迁移重构

## 兼容性

| 项目 | 版本 |
|------|------|
| ClassIsland | ≥ 2.0.0.0 |
| 插件 API | 2.0.0.0 |
