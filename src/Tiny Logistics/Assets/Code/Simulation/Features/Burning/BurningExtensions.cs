namespace Code.Simulation.Features.Burning {
    public static class BurningExtensions {
        public static void StopBurning(this GameEntity entity) {
            entity.isBurning = false;

            if (entity.hasBurningTimeLeft)
                entity.RemoveBurningTimeLeft();

            if (entity.hasBurnDamagePerSecond)
                entity.RemoveBurnDamagePerSecond();
        }
    }
}
