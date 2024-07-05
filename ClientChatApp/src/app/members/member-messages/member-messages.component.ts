import { ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm!: NgForm;
  @Input() messages!: Message[];
  @Input() username!: string;

  //loading: boolean=false;
  messageContent!: string;

  scrollTop!: number;
  constructor(private messageService: MessageService ,private cdr: ChangeDetectorRef) {}
  ngOnInit(): void {
 
  }

  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).subscribe((message) => {

      this.messages.push(message);
      this.messageForm.reset();
      
  })
}

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe(()=>{
      this.messages.splice(this.messages.findIndex(m=>m.id ===id),1)
    });
  }
}

