import socket
import threading
import time
import csv

def send(sock, csv_file_path):
    with open(csv_file_path, newline='', encoding='utf-8') as csvfile:
        csvreader = csv.reader(csvfile, delimiter=',')
        for row in csvreader:
            sendingData = ','.join(row)  # CSV 파일의 한 줄을 ','로 구분하여 문자열로 만듦
            sock.send(sendingData.encode('utf-8'))
            time.sleep(1)  # 메시지를 보낸 후 잠시 대기

def receive(sock):
    while True:
        recvData = sock.recv(1024)  # 받는 값
        print(recvData.decode('utf-8'))

ip = '127.0.0.1'
port = 7777

clientSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
clientSock.connect((ip, port))

print('Connection has been made!')

csv_file_path = 'C:\\Users\\배현준\\Desktop\\scada-hmi(num).csv'  # CSV 파일 경로를 지정
sender = threading.Thread(target=send, args=(clientSock, csv_file_path))
receiver = threading.Thread(target=receive, args=(clientSock,))

sender.start()
receiver.start()

try:
    while True:
        time.sleep(1)
except KeyboardInterrupt:
    print('Program terminated!')
finally:
    clientSock.close()
