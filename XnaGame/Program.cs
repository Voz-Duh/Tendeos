#if RELEASE
using XnaGame.Utils;
#endif

using var game = new XnaGame.Core();

#if RELEASE
Debug.Create();
Debug.Safe(game, game.Run);
Debug.Destroy();
Debug.WaitForDone();
#else
game.Run();
#endif
