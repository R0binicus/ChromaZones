public enum EventType
{
    LOSE,                   // Broadcasts when game is lost
    WIN,                    // Broadcasts when game is won
    RESTART_LEVEL,          
    NEXT_LEVEL,             
    QUIT_LEVEL,             
    SCENE_LOAD,             
    ADD_ENEMY,              // Broadcasts an instantiated Enemy to the EnemyManager to add to its list
    INIT_COLOUR_MANAGER,    // 
    COLOUR_CHANGE_BOOL,     // 
    ASSIGNMENT_CODE_TRIGGER,     // 
    AREA_CHASE_TRIGGER,     // 
    PAUSE_TOGGLE,
    PLAYER_MOVE_BOOL,
    PLAYER_MOVE_VECT2D,
    PLAYER_SPAWNPOINT,
    LEVEL_SELECTED,
    LEVEL_STARTED,
    LEVEL_ENDED,
    NEW_GAME_REQUEST,
    LOAD_GAME_REQUEST,
    LOAD_GAME_SUCCESS,
    LOAD_GAME_FAILED
};
