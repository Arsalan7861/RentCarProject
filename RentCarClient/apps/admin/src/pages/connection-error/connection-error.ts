import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-connection-error',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './connection-error.html',
})
export default class ConnectionErrorComponent {
  
}
