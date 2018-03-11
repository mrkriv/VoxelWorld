namespace GameCore.Entity
{
    public class Actor : Entity
    {
        public bool IsDeath { get; set; }
        
        public virtual void Kill()
        {
            OnDeath();
        }
        
        public virtual void OnDeath()
        {
            IsDeath = true;
        }
    }
}