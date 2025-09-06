import React from "react";
import type { User } from "../../types/users";

interface PersonInfoCardProps {
  title: string;
  person: User;
}

const PersonInfoCard: React.FC<PersonInfoCardProps> = ({ title, person }) => {
  return (
    <div className="detail-card">
      <h2>{title}</h2>
      <div className="person-info">
        <div className="person-avatar">
          {person.firstName.charAt(0)}
          {person.lastName.charAt(0)}
        </div>
        <div className="person-details">
          <h3>{person.fullName}</h3>
          <p>{person.email}</p>
          {person.phone && <p>{person.phone}</p>}
        </div>
      </div>
    </div>
  );
};

export default PersonInfoCard;
