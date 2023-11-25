public enum EventType
{
    LOSE,       // Broadcasts when game is lost
    WIN,        // Broadcasts when game is won
    ADD_ENEMY,  // Broadcasts an instantiated Enemy to the EnemyManager to add to its list
    RESTART_LEVEL,
    NEXT_LEVEL,
    QUIT_LEVEL
};
