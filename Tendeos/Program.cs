#if RELEASE
using Tendeos.Utils;

using var game = new Tendeos.Core();
Debug.Safe(game, game.Run);
Debug.ShowErrors();
#else
using var game = new Tendeos.Core();
game.Run();
#endif