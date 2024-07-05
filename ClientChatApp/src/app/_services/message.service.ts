import { Injectable, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root',
})
export class MessageService implements OnInit {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {}

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = this.getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);

    return this.getPaginatedResult<Message[]>(
      this.baseUrl + 'Messages',
      params
    );
  }
  deleteMessage(id:number) {
    return this.http.delete(this.baseUrl + 'Messages/'+ id);
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

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'Messages', {
      recipientUsername: username,
      content,
    });
  }
}
