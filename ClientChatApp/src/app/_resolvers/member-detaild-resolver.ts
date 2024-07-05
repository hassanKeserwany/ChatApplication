import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

import { member } from '../_models/member';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetaildResolver implements Resolve<member> {
  constructor(private memberService: MembersService) {}

  resolve(route: ActivatedRouteSnapshot):Observable<member> {
    return this.memberService.getMember(route.paramMap.get('username')!);
  }
}
