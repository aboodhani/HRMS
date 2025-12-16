import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgIf, NgFor, NgClass, NgStyle } from '@angular/common';
import { RandomColorDirective } from "./directives/random-color.directive";

// Decorator 
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NgIf, NgFor, NgClass, NgStyle, RandomColorDirective],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = "Welcome to Angular from Typescript";

  students =
    [
      
      { id : 1 , name: "stu1", mark: 49 },
      { id : 2, name: "stu2", mark: 23 },
      { id : 3 , name: "stu3", mark: 70 },
      { id : 4, name: "stu4", mark: 90 },
      { id : 5 , name: "stu5", mark: 55 }
    ]

  
}
