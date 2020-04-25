namespace ScoreFourServer.Domain
{
    public enum GameRoomStatus
    {
        /// <summary>
        /// After soon the game room created.
        /// </summary>
        Created,
        /// <summary>
        /// One of players moving at least once.
        /// </summary>
        Started,
        /// <summary>
        /// One of players has won.
        /// </summary>
        GameOver,
        /// <summary>
        /// One of players has left the game room.
        /// </summary>
        Left,
        /// <summary>
        /// Timed out
        /// </summary>
        Timedout,
    }
}