import { Injectable } from '@angular/core';
import { Service } from '../core/api.client';

@Injectable({
    providedIn: 'root'
})

export class AuthService {
    constructor(
        private apiService: Service
    ) { }

    getCurrentUser() {
        return JSON.parse(localStorage.getItem('currentUser'));
    }

    getUserType() {
        return this.getCurrentUser().role;
    }

    getToken(): string {
        return this.getCurrentUser().auth_token;
    }

    getDB(): string {
        return this.getCurrentUser().branchDB;
    }

    getUserName(): string {
        return this.getCurrentUser().userName;
    }

    getPassword(): string {
        return this.getCurrentUser().password;
    }

    isLoggedIn(): boolean {
        const data = this.getCurrentUser();
        return data && data.auth_token;
    }

    logout() {
        localStorage.removeItem('currentUser');
    }
}