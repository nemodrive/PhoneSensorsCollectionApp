from websocket_server import WebsocketServer
from argparse import ArgumentParser


# Called for every client connecting (after handshake)
def new_client(client, server):
    print("New client connected with id {} and address {}, {}".format(client['id'], client['address'][0], client['address'][1]))


# Called for every client disconnecting
def client_diconnected(client, server):
    print("Client(%d) disconnected" % client['id'])


# Called when a client sends a message
def message_received(client, server, message):
    print("Client(%d) said: %s" % (client['id'], message))


def run_server(ip_address, port):

    server = WebsocketServer(port, host=ip_address)
    server.set_fn_new_client(new_client)
    server.set_fn_client_left(client_diconnected)
    server.set_fn_message_received(message_received)
    server.run_forever()


if __name__ == '__main__':

    arg_parser = ArgumentParser()
    arg_parser.add_argument(
        "--ip",
        type=str,
        help="Ip address on which server listen")
    arg_parser.add_argument(
        "--port",
        type=int,
        help="Port on which server listen")
    args = arg_parser.parse_args()
    
    run_server(args.ip, args.port)