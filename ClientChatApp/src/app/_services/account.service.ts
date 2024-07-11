import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject, map } from 'rxjs';
import { User } from '../_models/User';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl: string = environment.apiUrl;
  //we will store the observable here , the type of observable is ReplaySubject,means
  //every time we subscribe to this observable, it emit the value inside it .
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'Account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    if(user){
      user.roles = [];
      const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles)?user.roles=roles: user.roles.push(roles);
    }
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next({
      username: '',
      token: '',
      photoUrl: '',
      knownAs: '',
      gender: '',
      roles:[],
    });
    window.location.reload();
  }
  registerService(modle: any) {
    return this.http.post<User>(this.baseUrl + 'Account/register', modle).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  getDecodedToken(token: string) {
    var result = JSON.parse(atob(token.split('.')[1]));
    console.log(result.role)
    console.log("result")

    return result;
  }
}
