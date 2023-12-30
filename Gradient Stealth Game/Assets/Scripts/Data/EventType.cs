public enum EventType
{
    LOSE,                   // Broadcasts when game is lost
    WIN,                    // Broadcasts when game is won     
    COLOUR_CHANGE_BOOL,     // 
    ASSIGNMENT_CODE_TRIGGER,     // 
    AREA_CHASE_TRIGGER,     // 
    PAUSE_TOGGLE,           // Broadcasts when pause is toggled on or off
    PLAYER_MOVE_BOOL,
    PLAYER_MOVE_VECT2D,
    REBUILD_NAVMESH,
    RESET_REGION_GAMEOBJECT_LINKS,

    //Gameobject inits
    ADD_ENEMY,              // Broadcasts an instantiated Enemy to the EnemyManager to add to its list
    INIT_COLOUR_MANAGER,    // Sends ColourManager reference
    INIT_PLAYER,            // Sends Player reference on game start
    INIT_PLAYER_REGION,     // Sends Player reference on level start

    // Scene and Level Management
    RESTART_LEVEL,          
    NEXT_LEVEL,             
    QUIT_LEVEL,             
    SCENE_LOAD,   
    FADING,
    SCENE_COUNT,
    PLAYER_SPAWNPOINT,
    LEVEL_SELECTED,
    LEVEL_STARTED,
    LEVEL_ENDED,
    QUIT_GAME,

    // Save/load game progress
    NEW_GAME_REQUEST,
    LOAD_GAME_REQUEST,
    LOAD_GAME_SUCCESS,
    LOAD_GAME_FAILED,
    SAVE_GAME,

    // Music and sound
    SFX,
    MUSIC,
    STOP_MUSIC,
    PAUSE_MUSIC,
    MUTEMUSIC_TOGGLE,
};