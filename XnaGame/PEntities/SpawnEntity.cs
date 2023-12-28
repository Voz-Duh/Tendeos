namespace XnaGame.PEntities
{
    public abstract class SpawnEntity : Entity
    {
        public SpawnEntity() => Core.AddEntity(Draw, Update);
    }
}
