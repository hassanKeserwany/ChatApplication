import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  FormGroup,
  Validators,
  ValidatorFn,
  AbstractControl,
  FormBuilder,
  Form,
} from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm!: FormGroup;
  maxDate!: Date;

  validationErrors:string[]=[];
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(15),
        ],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchValuePassword('password')],
      ],
    });

    this.registerForm.controls['password'].valueChanges.subscribe(() => {
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    });
  }

  matchValuePassword(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.get(matchTo)?.value
        ? null
        : { isMatching: true };
    };
  }

  register() {


    const formValue = { ...this.registerForm.value };

    // Format dateOfBirth to 'yyyy-MM-dd'
    if (formValue.dateOfBirth) {
      const date = new Date(formValue.dateOfBirth);
      formValue.dateOfBirth = date.toISOString().split('T')[0];
    }
  
    this.accountService.registerService(formValue).subscribe(
      (res) => {
        this.router.navigateByUrl('/members');
        
      },
      (error) => {
        this.validationErrors=error;
        console.log(error);
      }
    );
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}

