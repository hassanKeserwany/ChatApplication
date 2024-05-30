import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Form } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  model: any = {};
  @Output() cancelRegister = new EventEmitter();

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}
  ngOnInit(): void {}

  register(registerForm: any) {
    this.accountService.registerService(this.model).subscribe(
      (response) => {
        console.log('user added successfully');
        this.toastr.success('user added successfully ...');

        registerForm.reset(); //to reset the input
        this.cancel();
      },
      (error) => {
        console.log(error);
        this.toastr.error("Please add a valid info ...");
      }
    );
  }

  cancel() {
    //emit means send something
    this.cancelRegister.emit(false);
  }
}
