import { Injectable, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { map, take, tap } from 'rxjs/operators';
import { Message } from '../_models/message';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/User';
import { BehaviorSubject, Observable } from 'rxjs';
import { group } from '@angular/animations';
import { Group } from '../_models/group';
import { BusyService } from './busy.service';

@Injectable({
  providedIn: 'root',
})
export class MessageService implements OnInit {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();


  constructor(private http: HttpClient ,private busyService: BusyService) {}

  ngOnInit(): void {}

  createHubConnection(user: User, otherUsername: string) {
    this.busyService.busy();
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token,
        transport: HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(error => console.log(error))
      .finally(() => this.busyService.idle());
      

    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThreadSource.next(messages);
    })

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some(x => x.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages => {
            messages.forEach(message => {
              if (!message.dateRead) {
                message.dateRead = new Date(Date.now())
              }
            })
            this.messageThreadSource.next([...messages]);
          }
        })
      }
    })

    this.hubConnection.on('NewMessage', message => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: messages => {
          this.messageThreadSource.next([...messages, message])
        }
      })
    })
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.messageThreadSource.next([]);
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = this.getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);

    return this.getPaginatedResult<Message[]>(
      this.baseUrl + 'Messages',
      params
    );
  }
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'Messages/' + id);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'Messages/thread/' + username
    );
  }

  private getPaginationHeaders(
    pageNumber: number,
    pageSize: number
  ): HttpParams {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    return params;
  }

  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map((response) => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );
  }

  async sendMessage(username: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', {recipientUsername: username, content})
      .catch(error => console.log(error));
  }

//   // message.service.ts
// deleteConversationForUser(recepientUsername: string) {
//   return this.http.delete(this.baseUrl + `messages/delete-conversation/`+ recepientUsername);
//   this.messageThread$.subscribe(()=>{

//   })
// }


// message.service.ts
deleteConversationForUser(recipientUsername: string): Observable<void> {
  return this.http.delete<void>(`${this.baseUrl}messages/delete-conversation/${recipientUsername}`).pipe(
    tap(() => {
      // Notify subscribers that the message thread has been deleted
      this.messageThreadSource.next([]);
    })
  );
}



}
