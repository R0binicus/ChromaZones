public enum EventType
{
    LOSE,                   // Broadcasts when game is lost
    WIN,                    // Broadcasts when game is won
    RESTART_LEVEL,          
    NEXT_LEVEL,             
    QUIT_LEVEL,             
    SCENE_LOAD,             
    ADD_ENEMY,              // Broadcasts an instantiated Enemy to the EnemyManager to add to its list
    INIT_COLOUR_MANAGER,    // Sends ColourManager reference
    INIT_PLAYER,            // Sends Player reference
    COLOUR_CHANGE_BOOL,     // 
    ASSIGNMENT_CODE_TRIGGER,     // 
    AREA_CHASE_TRIGGER,     // 
    PAUSE_TOGGLE,           // Broadcasts when pause is toggled on or off
    PLAYER_MOVE_BOOL,
    PLAYER_MOVE_VECT2D,
    REBUILD_NAVMESH,
    SFX,
    MUSIC,
    STOP_MUSIC,
    PAUSE_MUSIC,
    PLAYER_SPAWNPOINT,
    LEVEL_SELECTED,
    LEVEL_STARTED,
    LEVEL_ENDED,
    NEW_GAME_REQUEST,
    LOAD_GAME_REQUEST,
    LOAD_GAME_SUCCESS,
    LOAD_GAME_FAILED,
    SAVE_GAME
};