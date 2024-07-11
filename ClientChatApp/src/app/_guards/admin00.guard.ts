import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class Admin00Guard implements CanActivate {

  constructor(private accountService :AccountService,
    private router:Router,
    private toastr:ToastrService) {}

  canActivate():Observable<any>{
    return this.accountService.currentUser$.pipe(
        map(user =>{
            if(user.roles.includes('Admin')||user.roles.includes('Moderator')){
                return true;
            }
            this.toastr.error('you cannot enter this area')
        })
    )
  }
    
}
