using System;
using Microsoft.Xna.Framework;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.SaveSystem;

namespace Tendeos.World
{
    public interface IMap
    {
        /// <summary>
        /// The width of the map in number of tiles.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the map in number of tiles.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The size of a single tile in pixels.
        /// </summary>
        float TileSize { get; }

        /// <summary>
        /// The size of a single chunk in number of tiles.
        /// </summary>
        int ChunkSize { get; }

        /// <summary>
        /// The width of the map in number of tiles including chunks that are outside of the viewable area.
        /// </summary>
        int FullWidth { get; }

        /// <summary>
        /// The height of the map in number of tiles including chunks that are outside of the viewable area.
        /// </summary>
        int FullHeight { get; }

        /// <summary>
        /// The used tile position.
        /// </summary>
        Vec2 UseTilePosition { get; }

        /// <summary>
        /// Indicates whether a tile is currently used.
        /// </summary>
        bool HasUsedTile { get; }

        /// <summary>
        /// Tries to place a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be placed.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        bool TryPlaceTile(bool top, ITile tile, int x, int y);

        /// <summary>
        /// Tries to place a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be placed.</param>
        /// <param name="position">The position of the tile.</param>
        bool TryPlaceTile(bool top, ITile tile, (int x, int y) position);

        /// <summary>
        /// Places a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be placed.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        bool PlaceTile(bool top, ITile tile, int x, int y);

        /// <summary>
        /// Places a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be placed.</param>
        /// <param name="position">The position of the tile.</param>
        bool PlaceTile(bool top, ITile tile, (int x, int y) position);

        /// <summary>
        /// Tries to set a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be set.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        bool TrySetTile(bool top, ITile tile, int x, int y);

        /// <summary>
        /// Tries to set a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be set.</param>
        /// <param name="position">The position of the tile.</param>
        bool TrySetTile(bool top, ITile tile, (int x, int y) position);

        /// <summary>
        /// Checks if the tile can be set at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        bool CanSetTile(bool top, int x, int y);

        /// <summary>
        /// Checks if the tile can be set at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        bool CanSetTile(bool top, (int x, int y) position);

        /// <summary>
        /// Checks if a tile can be placed at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <returns>True if the tile can be placed, false otherwise.</returns>
        public bool CanPlaceTile(bool top, int x, int y);

        /// <summary>
        /// Checks if a tile can be placed at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        /// <returns>True if the tile can be placed, false otherwise.</returns>
        public bool CanPlaceTile(bool top, (int x, int y) position);

        /// <summary>
        /// Uses a tile at the specified position.
        /// </summary>
        /// <param name="player">The player using the tile.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        void UseTile(Player player, int x, int y);

        /// <summary>
        /// Uses a tile at the specified position.
        /// </summary>
        /// <param name="player">The player using the tile.</param>
        /// <param name="position">The position of the tile.</param>
        void UseTile(Player player, (int x, int y) position);

        /// <summary>
        /// Tries to unuse the currently used tile.
        /// </summary>
        void TryUnuseTile();

        /// <summary>
        /// Mines a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <param name="power">The power of the explosion.</param>
        void MineTile(bool top, int x, int y, float power);

        /// <summary>
        /// Mines a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        /// <param name="power">The power of the explosion.</param>
        void MineTile(bool top, (int x, int y) position, float power);

        /// <summary>
        /// Mines a tile at the specified position with a specified radius.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <param name="power">The power of the explosion.</param>
        /// <param name="radius">The radius of the explosion.</param>
        void MineTile(bool top, int x, int y, float power, float radius);

        /// <summary>
        /// Mines a tile at the specified position with a specified radius.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        /// <param name="power">The power of the explosion.</param>
        /// <param name="radius">The radius of the explosion.</param>
        void MineTile(bool top, (int x, int y) position, float power, float radius);

        /// <summary>
        /// Sets a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be set.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        void SetTile(bool top, ITile tile, int x, int y);

        /// <summary>
        /// Sets a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="tile">The tile to be set.</param>
        /// <param name="position">The position of the tile.</param>
        void SetTile(bool top, ITile tile, (int x, int y) position);

        /// <summary>
        /// Destroys a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        void DestroyTile(bool top, int x, int y);

        /// <summary>
        /// Destroys a tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        void DestroyTile(bool top, (int x, int y) position);

        /// <summary>
        /// Sets the data of a tile at the specified position.
        /// </summary>
        /// <typeparam name="T">The type of the tile.</typeparam>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <param name="action">The action to be performed on the tile data.</param>
        void SetTileData<T>(bool top, int x, int y, Func<TileData, TileData> action) where T : ITile;

        /// <summary>
        /// Sets the data of a tile at the specified position.
        /// </summary>
        /// <typeparam name="T">The type of the tile.</typeparam>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        /// <param name="action">The action to be performed on the tile data.</param>
        void SetTileData<T>(bool top, (int x, int y) position, Func<TileData, TileData> action) where T : ITile;

        /// <summary>
        /// Flow the specified liquid to the given position.
        /// </summary>
        /// <param name="liquid">The liquid to apply flow to.</param>
        /// <param name="power">The intensity of the flow.</param>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <returns>The amount of flow applied.</returns>
        float Flow(Liquid.Liquid liquid, float power, int x, int y);

        /// <summary>
        /// Gets the chunk at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the chunk's position.</param>
        /// <param name="y">The y-coordinate of the chunk's position.</param>
        /// <returns>The chunk at the specified position.</returns>
        IChunk GetChunk(int x, int y);

        /// <summary>
        /// Gets the quadtree of the tile at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <returns>The quadtree of the tile at the specified position, or null if the tile does not exist.</returns>
        Rectangle? GetTileQuadtree(int x, int y);

        /// <summary>
        /// Gets the chunk of the tile at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <returns>The chunk of the tile at the specified position.</returns>
        IChunk GetTileChunk(int x, int y);

        /// <summary>
        /// Gets the data of the tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <returns>The data of the tile at the specified position.</returns>
        ref TileData GetTile(bool top, int x, int y);

        /// <summary>
        /// Gets the data of the tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        /// <returns>The data of the tile at the specified position.</returns>
        ref TileData GetTile(bool top, (int x, int y) position);

        /// <summary>
        /// Gets a non-referenced data of the tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <returns>The non-referenced copy of the data of the tile at the specified position.</returns>
        ref TileData GetUnrefTile(bool top, int x, int y);

        /// <summary>
        /// Gets a non-referenced data of the tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        /// <returns>The non-referenced copy of the data of the tile at the specified position.</returns>
        ref TileData GetUnrefTile(bool top, (int x, int y) position);

        /// <summary>
        /// Gets a non-referenced data of the tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="x">The x-coordinate of the tile's position.</param>
        /// <param name="y">The y-coordinate of the tile's position.</param>
        /// <returns>The non-referenced copy of the data of the tile at the specified position.</returns>
        /// <param name="ox">The x-coordinate of the tile's non-referenced data.</param>
        /// <param name="oy">The y-coordinate of the tile's non-referenced data.</param>
        ref TileData GetUnrefTile(bool top, int x, int y, out int ox, out int oy);

        /// <summary>
        /// Gets a non-referenced data of the tile at the specified position.
        /// </summary>
        /// <param name="top">Whether the tile is placed on the top or bottom of the map.</param>
        /// <param name="position">The position of the tile.</param>
        /// <returns>The non-referenced copy of the data of the tile at the specified position.</returns>
        /// <param name="ox">The x-coordinate of the tile's non-referenced data.</param>
        /// <param name="oy">The y-coordinate of the tile's non-referenced data.</param>
        ref TileData GetUnrefTile(bool top, (int x, int y) position, out int ox, out int oy);

        /// <summary>
        /// Converts the cell coordinates to chunk coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell's position.</param>
        /// <param name="y">The y-coordinate of the cell's position.</param>
        /// <returns>The chunk coordinates.</returns>
        (int x, int y) Cell2Chunk(int x, int y);

        /// <summary>
        /// Converts the cell coordinates to chunk coordinates.
        /// </summary>
        /// <param name="position">The position of the cell.</param>
        /// <returns>The chunk coordinates.</returns>
        (int x, int y) Cell2Chunk((int x, int y) position);

        /// <summary>
        /// Converts the world coordinates to cell coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the world's position.</param>
        /// <param name="y">The y-coordinate of the world's position.</param>
        /// <returns>The cell coordinates.</returns>
        (int x, int y) World2Cell(float x, float y);

        /// <summary>
        /// Converts the world coordinates to cell coordinates.
        /// </summary>
        /// <param name="position">The position of the world.</param>
        /// <returns>The cell coordinates.</returns>
        (int x, int y) World2Cell(Vec2 position);

        /// <summary>
        /// Converts the cell coordinates to world coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell's position.</param>
        /// <param name="y">The y-coordinate of the cell's position.</param>
        /// <returns>The world coordinates.</returns>
        Vec2 Cell2World(int x, int y);

        /// <summary>
        /// Converts the cell coordinates to world coordinates.
        /// </summary>
        /// <param name="position">The position of the cell.</param>
        /// <returns>The world coordinates.</returns>
        Vec2 Cell2World((int x, int y) position);

        /// <summary>
        /// Serializes the map to a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        [ToByte]
        void ToByte(ByteBuffer buffer);

        /// <summary>
        /// Deserializes the map from a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        [FromByte]
        void FromByte(ByteBuffer buffer);
    }
}