# Boss Setup Guide - Animator on Child Object

## Hierarchy Setup

Khi sử dụng Animator trên child object, hãy setup hierarchy như sau:

```
Boss (GameObject)
├── BossController.cs
├── DashAttack.cs
└── BossSprite (Child GameObject)
    ├── Animator (component)
    ├── SpriteRenderer (component)
    └── BossAnimation.cs
```

## Component Assignment

### BossController.cs
- **anim**: Có thể để trống, script sẽ tự động tìm Animator trong children
- **player**: Assign Transform của player
- **raycastOrigin**: Có thể để trống, sẽ dùng transform của Boss
- **dashAttack**: Có thể để trống, script sẽ tự động tạo component

### DashAttack.cs
- **anim**: Có thể để trống, script sẽ tự động tìm Animator trong children
- **player**: Sẽ được set tự động từ BossController
- Các settings khác có thể điều chỉnh theo ý muốn

### BossAnimation.cs (trên child object có Animator)
- Script sẽ tự động tìm BossController và DashAttack ở parent

## Animation Events

Trong Animator Controller, bạn cần setup các Animation Events:

### Melee Attack Animation
- **Event**: `EndAttack` (ở cuối animation)
- **Function**: Gọi `BossAnimation.EndAttack()`

### Dash Preparation Animation
- **Event**: `StartDash` (khi sẵn sàng bắt đầu dash)
- **Function**: Gọi `BossAnimation.StartDash()` (optional, chỉ để debug/effects)

### Fire Breath Animation  
- **Event**: `EndFireBreath` (ở cuối animation)
- **Function**: Gọi `BossAnimation.EndFireBreath()` (optional, chỉ để debug/effects)

## Animation Parameters

Animator Controller cần có các parameters sau:

### Triggers
- `attack` - Trigger melee attack
- `prepareDash` - Trigger prepare dash animation
- `dash` - Trigger dash animation
- `fireBreath` - Trigger fire breath animation

### Booleans
- `isAttacking` - True khi đang attack
- `isPreparingDash` - True khi đang chuẩn bị dash
- `isDashing` - True khi đang dash

## Auto-Detection

Cả 3 scripts (`BossController`, `DashAttack`, `BossAnimation`) đều có khả năng tự động tìm các component cần thiết:

- **BossController**: Tự động tìm Animator trong children
- **DashAttack**: Tự động tìm Animator trong children  
- **BossAnimation**: Tự động tìm BossController và DashAttack ở parent

## Debug Tips

1. **Kiểm tra Animator tìm đúng**: Xem console có thông báo "Animator is not found" không
2. **Kiểm tra Animation Events**: Xem console có log "Animation Event: ..." không
3. **Kiểm tra hierarchy**: Đảm bảo BossAnimation.cs ở cùng GameObject với Animator
4. **Kiểm tra Gizmos**: Chọn Boss object và xem Scene view có hiển thị các vòng tròn màu không

## Common Issues

### Issue: Animation Events không hoạt động
**Solution**: Đảm bảo BossAnimation.cs ở cùng GameObject với Animator

### Issue: Boss không tìm thấy Animator
**Solution**: Kiểm tra Animator có ở child object và script có GetComponentInChildren

### Issue: Dash không hoạt động  
**Solution**: Kiểm tra DashAttack component có được gán đúng trong BossController không
