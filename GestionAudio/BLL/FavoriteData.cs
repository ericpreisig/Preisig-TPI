using DTO;

namespace BLL
{
    /// <summary>
    /// Contain all action about favorit
    /// </summary>
    public static class FavoriteData
    {
        #region Public Methods

        /// <summary>
        /// Add an audio to favorite
        /// </summary>
        /// <param name="element"></param>
        public static void AddFavorite(Audio element)
        {
            element.IsFavorite = true;
            element.AddOrUpdateAudio();
        }

        /// <summary>
        /// remove an audio to favorite
        /// </summary>
        /// <param name="element"></param>
        public static void RemoveFavorite(Audio element)
        {
            element.IsFavorite = false;
            element.AddOrUpdateAudio();
        }

        #endregion Public Methods
    }
}