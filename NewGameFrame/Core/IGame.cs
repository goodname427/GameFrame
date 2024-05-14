namespace NewGameFrame.Core
{
    public interface IGame
    {
        /// <summary>
        /// 游戏阶段
        /// </summary>
        int Step { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 获得场景
        /// </summary>
        /// <param name="SceneIndex"></param>
        /// <returns></returns>
        Scene? GetScene(int SceneIndex);
    }
}
