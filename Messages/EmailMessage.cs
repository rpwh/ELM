﻿using ELM.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ELM
{
	public class EmailMessage : IMessageService
	{
		private string _header;
		private string _body;
		private HashSet<Tuple<string, string>> output = new HashSet<Tuple<string, string>>();
		public string sender;
		public string subject;
		public string message;

		public EmailMessage(String header, String body)
		{
			this._header = header;
			this._body = body;
		}

		public void Validate()
		{
			Regex email = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
			Match match = email.Match(sender);
			if (!match.Success)
			{
				throw new MessageValidationException("Invalid Email Address: " + sender);
			}

			if (sender.Length > 20)
			{
				throw new MessageValidationException("Email Address too long");
			}

			if (subject.Length > 140)
			{
				throw new MessageValidationException("Subject Length too long for Email Message");
			}

			if (message.Length > 1028)
			{
				throw new MessageValidationException("Message Length too long for Email Message");
			}
		}

		public EmailMessage SIRCheck()
		{
			return this;
		}

		public void ParseBody()
		{
			string body = this._body;
			try
			{
				//End of sender
				int x = body.ToLower().IndexOf("sender: ")+8;
				//Start of subject/end of sender text
				int y = body.ToLower().IndexOf("subject: ");
				//End of Subject identifier
				int z = body.ToLower().IndexOf("message: ");
				//Subject Start
				int d1 = y + 9;
				//Message Start
				int d2 = z + 9;

				this.sender = body.Substring(x, y - (x+1));
				this.subject = body.Substring(d1, z-(d1+1));
				this.message = body[d2..];
			}
			catch(Exception e)
			{
				//Raise this exception to MainWindow
				throw e;
			}
		}

		public void SanitiseBody()
		{
			string regex = @"(www|http:|https:)+[^\s]+[\w]";
			MatchCollection matches = Regex.Matches(message, regex);
			foreach(Match match in matches)
			{
				message= message.Replace(match.Value, "<URL Quarantined>");
			}
		}

		public HashSet<Tuple<string, string>> GetOutput()
		{
			output.Add(new Tuple<string, string>("sender", sender));
			output.Add(new Tuple<string, string>("subject", subject));
			output.Add(new Tuple<string, string>("message", message));
			return output;
		}

		public override string ToString()
		{
			return "Email Message - header: " + this._header + " body: " + this._body;
		}
	}
}
