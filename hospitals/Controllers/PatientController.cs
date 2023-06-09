﻿using hospitals.Models;
using hospitals.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hospitals.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="PatientController"/> class.
        /// </summary>
        /// <param name="patientRepository">The patient repository.</param>
        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
    
        /// <summary>
        /// Displays the list of patients.
        /// </summary>
        /// <returns>The task that represents the asynchronous operation.</returns>
        public async Task<IActionResult> Index()
        {
            var groupByRoom = Request.Query["filter_by_room"];
            var groupByDoc = Request.Query["filter_by_doc"];
            var groupByAddress = Request.Query["filter_by_address"];
            var minAge = Request.Query["older_than"];
            var maxAge = Request.Query["younger_than"];
            var showAddress = Request.Query["show_address"];
            var entryDate = Request.Query["entry_date"];
            var exitDate = Request.Query["exit_date"];
    
            ViewBag.MaxAge = await _patientRepository.GetMaxAgeAsync();
            ViewBag.MinAge = await _patientRepository.GetMinAgeAsync();
    
            var patients = await _patientRepository.GetAll();
    
            if (string.Equals(showAddress, "true"))
            {
                var patientsWithRoom = await _patientRepository.GetAllWithAddresses();
                return View("PatientsWithAddresses", patientsWithRoom);
            }
            if (string.Equals(groupByRoom, "true"))
            {
                var patientsByRoom = await _patientRepository.GetAllByRoom();
                return View("GrouppedByRoom", patientsByRoom);
            }
            if (string.Equals(groupByDoc, "true"))
            {
                var patientsByDoc = await _patientRepository.GetAllByDoctor();
                return View("GrouppedByDoctor", patientsByDoc);
            }
            if (string.Equals(groupByAddress, "true"))
            {
                var patientsByAddress = await _patientRepository.GetAllByAddress();
                return View("GrouppedByAddress", patientsByAddress);
            }
            if (!string.IsNullOrEmpty(minAge) && !string.IsNullOrEmpty(maxAge))
            {
                var olderThan = int.Parse(minAge);
                var youngerThan = int.Parse(maxAge);
                patients = await _patientRepository.GetPatientByAge(youngerThan, olderThan);
            }
            if (!string.IsNullOrEmpty(entryDate) && !string.IsNullOrEmpty(exitDate))
            {
                var enteredIn = DateTime.Parse(entryDate);
                var exitedIn = DateTime.Parse(exitDate);
                patients = await _patientRepository.GetPatientByDate(enteredIn, exitedIn);
            }
    
            return View(patients);
        }
    
        /// <summary>
        /// Displays the details of a specific patient.
        /// </summary>
        /// <param name="id">The ID of the patient.</param>
        /// <returns>The task that represents the asynchronous operation.</returns>
        public async Task<IActionResult> Detail(int id)
        {
            Patient patient = await _patientRepository.GetByIdAsync(id);
            return View(patient);
        }
    }
}
