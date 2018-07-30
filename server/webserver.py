import asyncio
import websockets

async def hello(websocket, path):
    while True:
        try:
            json = await websocket.recv()
            print(json)
        except websockets.ConnectionClosed:
            break

start_server = websockets.serve(hello, '127.0.0.1', 7070)

asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()
