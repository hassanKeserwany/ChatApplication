import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map, Observable } from 'rxjs';

export const adminGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | boolean => {
  // Inject the necessary services
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  // Return an observable to determine whether the route can be activated
  return accountService.currentUser$.pipe(
    map(user => {
      if (user.roles.includes('Admin') || user.roles.includes('Moderator')) {
        toastr.success('You are admin');
        return true;
      }
      toastr.error('You cannot enter this area');
      return false;
    })
  );
};
