#if RELEASE
using XnaGame.Utils;
#endif

using var game = new XnaGame.Core();

#if RELEASE
Debug.Safe(game, game.Run);
Debug.WaitForDone();
#else
game.Run();
#endif
