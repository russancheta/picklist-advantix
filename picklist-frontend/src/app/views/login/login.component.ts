import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { AuthService } from '../../shared/auth.service';
import { Service, CredentialsViewModel } from '../../core/api.client';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'login.component.html',
  styleUrls: ['login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  dbName: string = '';
  loading = false;
  submitted = false;
  returnUrl: string;
  error = '';
  showPassBtnLabel = 'Show';
  
  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private apiService: Service
  ) { }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.authService.logout();

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  showPassword(input: any): any {
    input.type = input.type === 'password' ? 'text' : 'password';
  }

  get f() { return this.loginForm.controls; }

  onSubmit() {
    this.submitted = true;

    if (this.loginForm.invalid) {
      return;
    }
 
    this.loading = true;
    const credential = new CredentialsViewModel();
    credential.dbName = this.dbName;
    credential.userName = this.f.username.value;
    credential.password = this.f.password.value;
    credential.address = '10.0.100.32';
    credential.dbName = 'ZZ_TESTPOSTING';
    this.apiService.login(credential)
      .subscribe(
        response => {
          if (response.result === 'Success') {
            localStorage.setItem('currentUser', JSON.stringify(response.resultData));
            this.router.navigate(['pickandpack']);
            const toast = Swal.mixin({
              toast: true,
              position: 'top-end',
              showConfirmButton: false,
              timer: 3000
            });
 
            toast.fire({
              type: 'success',
              title: 'Signed in successfully'
            });
          } else {
            this.error = response.message;
          }
          this.loading = false;
        },
        error => {
          this.error = error;
          this.loading = false;
      }
    );
  }
  
}
