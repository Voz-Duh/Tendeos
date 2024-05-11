using var game = new Tendeos.Core();

#if RELEASE
using Tendeos.Utils;

Debug.Safe(game, game.Run);
Debug.WaitForDone();
#else
game.Run();
#endif
