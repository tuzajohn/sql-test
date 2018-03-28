#include<stdio.h> //printf
#include<string.h> //memset
#include<stdlib.h> //exit(0);
#include<arpa/inet.h>
#include<sys/socket.h>
#include <unistd.h>
#include <stdbool.h>

typedef struct {
    unsigned int length;
    unsigned char data[1000];
}Message;
typedef enum{
    OK,
    BAD,
    WRONGLENGTH,
    DivZero
} Status;
typedef struct sockaddr_in SocketAddress;
//excercise 2
typedef struct{
    enum{Request, Reply} MessageType;
    unsigned int RPCId, ProcedureId;
    int arg1, arg2;
}RPCMessage;

int arg1(char msg[], int index){
    char a[10];
    for(int i = 0; i < index; i++){
        if(msg[i] != ' '){
            a[i] = msg[i];
        }
    }
    int num = atoi(a);
    return  num;
}
int arg2(char* msg, int index){
    char a[10];
    for(int i = index; i < strlen(msg); i++){
        if(msg[index+1] != ' '){
            a[i - index] = msg[i+1];
        }
    }
    //printf("\n -- number[%s] --", a);
    int num = atoi(a);
    return  num;
}

Status Op(RPCMessage *rm, int arg1, int arg2, int* ans){
    switch (rm->ProcedureId){
        case 1: *ans = arg1 + arg2;
            return OK;
        case 2: *ans = arg1 - arg2;
            return OK;
        case 3: *ans = arg1 * arg2;
            return OK;
        case 4:
            if(rm->arg2 != 0){
                *ans = arg1 / arg2;
                return OK;
            }else{
                *ans = 000;
                return DivZero;
            }
        default:
            return BAD;
    }
}

void unMarshall(RPCMessage *rm, Message *message){
    char msg[sizeof(message)];
    strcpy(msg, message);
    for(int i = 0; i < strlen(msg); i++){
        if(msg[i] == '+'){
            rm->ProcedureId = 1;
            rm->arg1 = arg1(msg, i);
            rm->arg2 = arg2(msg, i);
        }
        else if(msg[i] == '-'){
            rm->ProcedureId = 2;
            rm->arg1 = arg1(msg, i);
            rm->arg2 = arg2(msg, i);
        }
        else if(msg[i] == '*'){
            rm->ProcedureId = 3;
            rm->arg1 = arg1(msg, i);
            rm->arg2 = arg2(msg, i);
        }
        else if(msg[i] == '/'){
            rm->ProcedureId = 4;
            rm->arg1 = arg1(msg, i);
            rm->arg2 = arg2(msg, i);
        }
        else{
            //rm->ProcedureId = 0;
        }
    }
    int ans;
    Status o = Op(rm,rm->arg1, rm->arg2, &ans);
    //memset(message,'\0',strlen(message));
    if(o == BAD){
        char temp[1000] = "Status [Bad Request]";
        strcpy(message,temp);
    }
    else if(o == DivZero){
        char temp[1000] = "Status [DivZero]";
        strcpy(message,temp);
    }
    else{
        char temp[100] = "Status [OK]\nThe answer to [ ", str1[30], str2[30] = " ] is [ ", final[30];
        strcpy(str1, message);
        snprintf(final, sizeof(final), "%d", ans);
        strcat(temp, str1);
        strcat(temp, str2);
        strcat(temp, final);
        strcat(temp, " ]");
        char string[strlen(temp)];
        strcpy(string, temp);
        strcpy(message,string);//outputs the answer
    }
}
void die(char *s)//Custom error message
{
    perror(s);
    exit(1);
}

int main(void)
{
    struct sockaddr_in si_me, clientSA;
    int s, port, Id = 0;
    char buf[1024], id[20];
    //setting the port for discussion
    printf("Setting PORT for conversation:\n");
    scanf("%d", &port);
    getchar();

    //create a UDP socket
    if ((s=socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == -1){ die("socket"); }
    // zero out the structure
    memset((char *) &si_me, 0, sizeof(si_me));
    si_me.sin_family = AF_INET;
    si_me.sin_port = htons(port);
    si_me.sin_addr.s_addr = htonl(INADDR_ANY);
    //bind socket to port
    if( bind(s , (struct sockaddr*)&si_me, sizeof(si_me) ) == -1){ die("bind"); }
    RPCMessage* ms;
    //keep listening for data
    while(1)
    {
        printf("Waiting for data...\n");
        fflush(stdout);
        //clear the buffer by filling null, it might have previously received data
        memset(buf,'\0', 1024);
        int slen = sizeof(clientSA);
        if ((recvfrom(s, buf, 1024, 0, (struct sockaddr *) &clientSA, &slen)) == -1){die("recvfrom()");}//receive data
        Id ++;
        ms->RPCId = Id;
        //try to receive some data, this is a blocking call
        //print details of the client.
        printf("Received packet from %s:%d\n", inet_ntoa(clientSA.sin_addr), ntohs(clientSA.sin_port));
        printf("Request: %s\n" , buf);//print data received by the client.
        for(int i=0; i < strlen(buf); i++){
            if(buf[i] == '+' || buf[i] == '-' || buf[i] == '*' || buf[i] == '/'){
                unMarshall(ms, &buf);//performs the arithmetic then stores in the buffer
                break;
            }
        }
        if(buf[0] == 'q' || (strstr(buf, "Stop") != NULL)){//check if input is stopping character!
            char msg[] = "You terminated the conncection\nStatus [OK]";//now reply the client with the custom message.
            if (sendto(s, msg, strlen(msg), 0, (struct sockaddr*) &clientSA, slen) == -1){//sends data to client
                die("sendto()");
            }
            printf("process terminated by client");
            break;
        }
        char temp[1000];
        strcpy(temp, "SERVER: Request received with id:[ ");
        snprintf(id, id, "%d", ms->RPCId);
        strcat(temp, id);
        strcat(temp, " ]!\nSERVER: Response from Server [ ");
        strcat(temp, buf);
        strcat(temp, " ]!");
        //now reply the client with the some data
        if (sendto(s, temp, strlen(temp), 0, (struct sockaddr*) &clientSA, slen) == -1){//sends data to client
            die("sendto()");
        }
    }
    close(s);
    return 0;
}
