# Dash Fire Combo System - Simplified

## Tổng quan
Đã đơn giản hóa hệ thống Boss bằng cách loại bỏ các attack riêng lẻ và chỉ sử dụng combo "Dash + Fire Breath" thông qua component `DashAttack`.

## Cách hoạt động

### BossController - Simplified
**Chỉ còn 3 states:**
- `Idle`: Trạng thái chờ
- `Attacking`: Tấn công cận chiến
- `DashFireCombo`: Thực hiện combo dash + fire breath

### Attack Logic
**Khoảng cách tấn công:**
- **≤ 3f**: Melee Attack (cận chiến)
- **> 3f và ≤ 10f**: Dash Fire Combo
- **> 10f**: Idle (quá xa)

### Quy trình Combo (Không thay đổi)
1. **Chuẩn bị**: Boss quay mặt về player và trigger animation "prepareDash"
2. **Dash**: Boss dash đến vị trí cách player 4f units
3. **Delay**: Nghỉ ngắn để chuẩn bị phun lửa
4. **Fire Breath**: Boss quay mặt về player và trigger animation "fireBreath"
5. **Hoàn thành**: Reset tất cả flags và thông báo cho BossController

## Những gì đã loại bỏ

### Từ BossController
❌ **States đã xóa:**
- `BreathingFire`
- `PreparingDash` 
- `Dashing`

❌ **Variables đã xóa:**
- `fireBreathRange`
- `dashRange`
- `dashPrepareTime`
- `dashSpeed`
- `dashDuration`
- `lastDashTime`
- `dashStartTime`
- `dashTarget`
- `dashStartPosition`

❌ **Methods đã xóa:**
- `CanDash()`
- `PrepareDash()`
- `StartDash()`
- `PerformDash()`
- `EndDash()`
- `TriggerBreath()`

## Setup trong Unity
1. Đảm bảo GameObject có component `DashAttack`
2. Assign các references cần thiết trong BossController:
   - `dashAttack`: Reference đến DashAttack component
3. Các animations cần có:
   - `prepareDash` trigger
   - `dash` trigger  
   - `fireBreath` trigger
   - `attack` trigger (cho melee)
   - `isPreparingDash` bool
   - `isDashing` bool
   - `isAttacking` bool

## Gizmos Debug - Simplified
- **Vàng**: Melee attack range (3f)
- **Magenta**: Dash fire combo range (10f)
- **Xanh lá**: Raycast đến player (có thể nhìn thấy)
- **Vàng**: Raycast đến player (bị cản)

## Parameters còn lại

### BossController
- `meleeAttackRange` = 3f
- `dashFireComboRange` = 10f  
- `dashCooldown` = 3f
- `idleDuration` = 1f

### DashAttack (Không thay đổi)
- `dashSpeed` = 15f
- `dashDuration` = 0.5f
- `fireBreathRange` = 4f (cố định)
- `fireBreathDelay` = 0.2f

## Lợi ích của việc đơn giản hóa
✅ **Code gọn gàng hơn**: Ít states, ít variables, ít methods
✅ **Dễ maintain**: Logic tập trung vào DashAttack component
✅ **Flexible**: Có thể dễ dàng thêm attack types mới
✅ **Clear separation**: BossController chỉ lo AI logic, DashAttack lo combo execution
✅ **Reusable**: DashAttack có thể được sử dụng cho boss khác
