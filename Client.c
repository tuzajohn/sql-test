/*
    Simple udp client
*/
#include<stdio.h> //printf
#include<string.h> //memset
#include<stdlib.h> //exit(0);
#include<arpa/inet.h>
#include<sys/socket.h>
#include <unistd.h>
#include <signal.h>
#include <errno.h>

typedef struct {
    unsigned int length;
    unsigned char data[1000];
}Message;
typedef enum{
    OK,
    BAD,
    WRONGLENGTH
} Status;
typedef struct sockaddr_in SocketAddress;

void die(char *s)//Custom error message
{
    perror(s);
    exit(1);
}

Status DoOperation(Message *message, Message *reply,int s, SocketAddress serverSA);
int main(){
    struct sockaddr_in si_other;
    int s, port;
    char buf[1024], replBuf[1024], message[1024];
    char server[50], client[1074];
    printf("--- Input SERVER address ---\n");
    scanf("%s", &server);
    printf("--- Communication PORT --- \n");
    scanf("%d", &port);
    getchar();

    if ( (s=socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == -1)//creates a CLient UDP socket
    {
        die("socket");
    }

    si_other.sin_family = AF_INET;
    si_other.sin_port = htons(port);

    if (inet_aton(server , &si_other.sin_addr) == 0)
    {
        fprintf(stderr, "inet_aton() failed\n");
        exit(1);
    }

    while(1)
    {
        printf("Client says : ");
        scanf("%[^\n]%*c", &buf);//stores a complete string
        if(strlen(buf) > 1000){//lenth of message is checked
            printf("\n\n -- -- Status [WRONGLENGTH] -- -- \n\n");
        }
        else{
            if(buf[0] == 'q'|| (strstr(buf, "Stop") != NULL)){
                Status sn = DoOperation(buf, replBuf,s, si_other);
                if(sn == BAD){printf("Bad client request: Status[BAD]\n\n");}
                else if(sn == WRONGLENGTH){printf("Wrong length from client request: Status[WRONGLENGTH]\n\n");}
                else{printf("Successful client request: Status[OK]\n\n");}
                break;
            }
            else{
                Status sn = DoOperation(buf, replBuf,s, si_other);
                if(sn == BAD){printf("Bad client request: Status[BAD]\n\n");}
                else if(sn == WRONGLENGTH){printf("Wrong length from client request: Status[WRONGLENGTH]\n\n");}
                else{printf("Successful client request: Status[OK]\n\n");}
            }
        }
        memset(buf, 0, sizeof(buf));//clears the buffer for next Client request
    }
    close(s);//Closes the program
    return 0;
}
Status DoOperation(Message *message, Message *reply,int s, SocketAddress serverSA){

    alarm(1000);
    int sendId = sendto(s, message, strlen(message), 0, (struct sockaddr *)&serverSA, sizeof(serverSA));
    Status sn = OK;
    if(sendId < 0){ sn = BAD; die("sendto()"); }

    memset(message,'\0', 1000);
    int slen = sizeof(serverSA);
    if (recvfrom(s, reply, 1000, 0, (struct sockaddr *) &serverSA, &slen) < 0){
        if(errno == EINTR){ printf("Server took too long to reply"); }
        else{
            die("recvfrom()");
        }
    }
    puts(reply);
    memset(reply,'\0', strlen(reply));
    return  sn;
}