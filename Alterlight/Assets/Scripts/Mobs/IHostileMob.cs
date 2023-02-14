using System;

public interface IHostileMob
{
    void GetMousePosition();
    void HandleDamage();
    void HandleMovement();
    void DealDamage(PlayerMovement player);
    void PathFind(PlayerMovement player);
}