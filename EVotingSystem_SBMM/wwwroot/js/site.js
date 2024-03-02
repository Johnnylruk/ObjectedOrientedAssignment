/*Alert button*/
$('.close-alert').click(function () {
    $('.alert').hide('hide');
});

$(document).ready(function () {
    $('#tables').DataTable({
        "columnDefs": [{
            "targets": "_all", // Target all columns
            "className": "text-center" // Apply 'text-center' class to center align content
        }]
    });

    $('.btn-approve-voter, .btn-deny-voter').click(function (){
        var voterId = $(this).attr('data-voter-id');
        $('.btn-save-changes').data('voterId', voterId); // store voterId in save button
        if($(this).hasClass('btn-approve-voter')){
            $('#pendingApprovalModal').modal();
        } else {
            $('#pendingDenyModal').modal();
        }
    });

    $('.btn-save-changes').click(function (){
        var voterId = $(this).data('voterId'); // retrieve voterId from save button
        $.ajax({
            url: approveVotersUrl,
            type: 'POST',
            dataType: 'json',
            data: { voterId: voterId },
            success: function (response) {
                $('#pendingApprovalModal').modal('hide');
                $('#pendingDenyModal').modal('hide');
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    });
});


//Voting SPTP redirecting to other page before click vote
function selectCandidate(candidateId, name, party, description) {
    // Update the Selected Candidate section with the selected candidate's information
    var selectedCandidateSection = document.getElementById("selectedCandidateSection");
    selectedCandidateSection.innerHTML = `
        <div>
            <strong>Name:</strong> ${name}<br />
            <strong>Party:</strong> ${party}<br />
            <strong>Description:</strong> ${description}<br />
        </div>
    `;

    // Set the selected candidate ID in a hidden input field
    var selectedCandidateForm = document.createElement("form");
    selectedCandidateForm.method = "post";
    selectedCandidateForm.action = "/Vote/SubmitVoteFPTP"; 
    var candidateIdInput = document.createElement("input");
    candidateIdInput.type = "hidden";
    candidateIdInput.name = "candidateId";
    candidateIdInput.value = candidateId;
    selectedCandidateForm.appendChild(candidateIdInput);
    var voteButton = document.createElement("button");
    var backButton = document.createElement("button");
    voteButton.type = "submit";
    voteButton.className = "btn btn-success";
    voteButton.textContent = "Vote";
    backButton.addEventListener("click", function (){
        window.location.href = "/AccessVoter/Ballot";
    });
    backButton.type = "button";
    backButton.className = "btn btn-secondary";
    backButton.textContent = "Back";
    selectedCandidateForm.appendChild(voteButton);
    selectedCandidateForm.appendChild(backButton);
    selectedCandidateSection.appendChild(selectedCandidateForm);
    
}

//Voting STV Rank Logic
$(document).ready(function () {
    $('.rank-selectSTV').change(function () {
        var selectedRank = $(this).val();
        $('.rank-selectSTV option').prop('disabled', false);
        $('.rank-selectSTV').not(this).find('option[value="' + selectedRank + '"]').prop('disabled', true);
    });
});


//Voting PV Rank Logic
$(document).ready(function () {
    $('.rank-selectPV').change(function () {
        var selectedRank = $(this).val();
        $('.rank-selectPV option').show(); // Show all options
        $('.rank-selectPV').not(this).find('option[value="' + selectedRank + '"]').hide(); // Hide selected option for other candidates

        // Check for equality of ranks
        var equalRanks = false;
        $('.rank-selectPV').each(function () {
            if ($(this).val() !== '' && $(this).val() === selectedRank && !$(this).is(':focus')) {
                equalRanks = true;
                return false; // Exit the loop early if equal ranks found
            }
        });

        // Disable the vote button and display error message if equal ranks found
        if (equalRanks) {
            $('#voteButton').prop('disabled', true);
            $('#errorMessage').text('Rank numbers cannot be equal').show();
        } else {
            $('#errorMessage').hide(); // Hide error message if no equal ranks found
        }

        // Check if all rank-select dropdowns have been selected
        var allRanked = true;
        $('.rank-selectPV').each(function () {
            if ($(this).val() === '') {
                allRanked = false;
                return false; // Exit the loop early if any dropdown is not selected
            }
        });

        // Enable or disable the vote button based on whether all candidates have been ranked and no equal ranks found
        if (allRanked && !equalRanks) {
            $('#voteButton').prop('disabled', false);
        } else {
            $('#voteButton').prop('disabled', true);
        }
    });

    // Disable the vote button initially
    $('#voteButton').prop('disabled', true);
});

//SideBar
document.addEventListener("DOMContentLoaded", function (event) {

    const showNavbar = (toggleId, navId, bodyId, headerId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId)

        // Validate that all variables exist
        if (toggle && nav && bodypd && headerpd) {
            toggle.addEventListener('click', () => {
                // show navbar
                nav.classList.toggle('show')
                // change icon
                toggle.classList.toggle('bx-x')
                // add padding to body
                bodypd.classList.toggle('body-pd')
                // add padding to header
                headerpd.classList.toggle('body-pd')
            })
        }
    }

    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

    /*===== LINK ACTIVE =====*/
    const linkColor = document.querySelectorAll('.nav_link')

    function colorLink() {
        if (linkColor) {
            linkColor.forEach(l => l.classList.remove('active'))
            this.classList.add('active')
        }
    }
    linkColor.forEach(l => l.addEventListener('click', colorLink))
});

