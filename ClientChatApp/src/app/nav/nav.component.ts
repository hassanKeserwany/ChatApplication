import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { account } from '../_models/account';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})

export class NavComponent implements OnInit {
  model: account = {
    username: '',
    password: '',
  };

  //get user form local storage
  userNameFormStorage: string = '';

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe(
      (response) => {
        this.router.navigateByUrl('/members');
        this.toastr.success('Logging successfully ...');
      },
      (error) => {
        console.log(error);
        this.toastr.error('Invalid username or password . . .');
      }
    );
  }
  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
