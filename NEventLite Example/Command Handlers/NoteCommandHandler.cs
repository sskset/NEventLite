﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventLite.Command_Handlers;
using NEventLite.Exceptions;
using NEventLite.Repository;
using NEventLite_Example.Commands;
using NEventLite_Example.Domain;

namespace NEventLite_Example.Command_Handlers
{
    public class NoteCommandHandler :
        ICommandHandler<CreateNoteCommand>, 
        ICommandHandler<EditNoteCommand>
    {
        private readonly IRepository<Note> _repository;

        public NoteCommandHandler(IRepository<Note> repository)
        {
            _repository = repository;
        }

        public void Handle(CreateNoteCommand command)
        {
            var newNote = new Note(command.title, command.desc, command.cat);
            _repository.Save(newNote);
        }

        public void Handle(EditNoteCommand command)
        {
            var LoadedNote = _repository.GetById(command.AggregateId);

            if (LoadedNote != null)
            {
                if (LoadedNote.CurrentVersion == command.TargetVersion)
                {
                    if (LoadedNote.Title != command.title)
                        LoadedNote.ChangeTitle(command.title);

                    if (LoadedNote.Category != command.cat)
                        LoadedNote.ChangeCategory(command.cat);
                }
                else
                {
                    throw new ConcurrencyException($"The version of the Note ({LoadedNote.CurrentVersion}) and Command ({command.TargetVersion}) didn't match.");
                }
            }
            else
            {
                throw new AggregateNotFoundException($"Note with ID {command.AggregateId} was not found.");
            }
        }
    }
}
