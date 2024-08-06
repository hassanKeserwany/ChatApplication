import { Injectable, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { member } from '../_models/member';
import { catchError, map, of, take, tap, throwError } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root',
})
export class MembersService implements OnInit {
  baseUrl = environment.apiUrl;
  members: member[] = [];
  paginatedResult: PaginatedResult<member[]> = new PaginatedResult<member[]>();
  memberCache = new Map();
  user!: User;
  userParams!: UserParams;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((res) => {
      this.user = res;
      this.userParams = new UserParams(); // i can send user if i want to change gender
    });
  }
  ngOnInit(): void {}

  addlikes(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }
  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = this.getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return this.getPaginatedResult<Partial<member[]>>(this.baseUrl+'Likes',params)
}





  getUserParams() {
    return this.userParams;
  }
  setUserParams(params: UserParams) {
    this.userParams = params;
  }
  resetUserParams() {
    this.userParams = new UserParams();
    return this.userParams;
  }
  getMembers(userParams: UserParams) {
    const cacheKey = Object.values(userParams).join('-');
    var cachedResponse = this.memberCache.get(cacheKey);

    if (cachedResponse) {
      return of(cachedResponse);
    }

    let params = this.getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    if (userParams.gender) {
      params = params.append('gender', userParams.gender);
    }
    params = params.append('orderBy', userParams.orderBy.toString());

    return this.getPaginatedResult<member[]>(
      this.baseUrl + 'users',
      params
    ).pipe(
      map((response) => {
        this.memberCache.set(cacheKey, response);
        return response;
      })
    );
  }

  getMember(username: string) {
    //inorder to cache all the members in one array , use reduce
    const member = [...this.memberCache.values()]
      .reduce((arr, ele) => arr.concat(ele.result), [])
      .find((member: member) => member.username === username);
    if (member) {
      return of(member);
    }

    return this.http.get<member>(this.baseUrl + 'Users/username/' + username);
  }
  // updateMember(member: member) {
  //   return this.http.put(this.baseUrl + 'users', member).pipe(
  //     map(() => {
  //       const index = this.members.indexOf(member);
  //       this.members[index] = member;
  //     })
  //   );
  // }

  updateMember(member: member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.findIndex(m => m.id === member.id);
        if (index !== -1) {
          this.members = [
            ...this.members.slice(0, index),
            member,
            ...this.members.slice(index + 1)
          ];
        }
      }),
      catchError(error => {
        console.error('Update member failed', error);
        return throwError(error);
      })
    );
  }
  

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  setMainToPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
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

  getPaginatedResult<T>(url: string, params: HttpParams) {
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
}
