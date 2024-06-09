import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountService :AccountService,
    private router:Router,
    private toastr:ToastrService) {}

  canActivate():Observable<any>{
    return this.accountService.currentUser$.pipe(
      map(user =>{
        if(user)return true;

        this.router.navigate(['/login']);
       return this.toastr.error('you may not pass !')
      })
    );
  }
    
}
