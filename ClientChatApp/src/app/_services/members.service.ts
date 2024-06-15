import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: member[] = [];
  constructor(private http: HttpClient) {}

  getMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<member[]>(this.baseUrl + 'Users')
    .pipe(
      map(res=>{
        this.members=res;
        return this.members;
      })
    )
    
  }
  getMember(username: string) {
    const member=this.members.find(x=>x.username === username);
    if(member !==undefined) return of(member);
    return this.http.get<member>(this.baseUrl + 'Users/username/' + username);
  }
  updateMember(member: member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(()=>{
        const index =this.members.indexOf(member);
        this.members[index]=member;
      })
    )

  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  setMainToPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId ,{})
  }


}
