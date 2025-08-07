import { inject } from '@angular/core';
import { CanActivateChildFn, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Common } from '../services/common';

export const authGuard: CanActivateChildFn = (childRoute, state) => {
  const token = localStorage.getItem('response');
  const router = inject(Router);
  const common = inject(Common);
  if (!token) {
    router.navigateByUrl('login');
    return false;
  }

  try {
    const decode: any = jwtDecode(token);

    // Create a new decode object to update the signal properly
    const newDecodeData = {
      id: decode[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ],
      fullName: decode['fullName'],
      fullNameWithEmail: decode['fullNameWithEmail'],
      email: decode['email'],
      role: decode['role'],
      permissions: JSON.parse(decode['permissions']),
      branch: decode['branch'],
      branchId: decode['branchId'],
      tfaStatus:
        decode['tfaStatus'] === 'true' || decode['tfaStatus'] === true || false,
    };

    // Update the signal with the new object
    common.decode.set(newDecodeData);

    const now = new Date().getTime() / 1000; // Current time in seconds
    const exp = decode.exp ?? 0; // Expiration time from the token
    if (exp <= now) {
      router.navigateByUrl('login');
      return false;
    }
    return true;
  } catch (error) {
    router.navigateByUrl('login');
    return false;
  }
};
